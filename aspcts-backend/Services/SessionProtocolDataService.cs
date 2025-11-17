using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Models.Entities;
using aspcts_backend.Data;

namespace aspcts_backend.Services
{
    public class SessionProtocolDataService : ISessionProtocolDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISessionRepository _sessionRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;

        public SessionProtocolDataService(
            ApplicationDbContext context,
            ISessionRepository sessionRepository,
            IChildRepository childRepository,
            IUserRepository userRepository)
        {
            _context = context;
            _sessionRepository = sessionRepository;
            _childRepository = childRepository;
            _userRepository = userRepository;
        }

        public async Task<SessionProtocolDataResponse?> GetBySessionIdAsync(Guid sessionId, Guid userId, string userRole)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return null;

            var canAccess = await CanAccessChild(session.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            var data = await _context.SessionProtocolData
                .Include(spd => spd.Records)
                    .ThenInclude(r => r.Intervals)
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);

            if (data == null)
                return null;

            return MapToResponse(data);
        }

        public async Task<SessionProtocolDataResponse> CreateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                throw new ArgumentException("Sessão não encontrada ou acesso negado");

            var existing = await _context.SessionProtocolData
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);
            
            if (existing != null)
                throw new InvalidOperationException("Dados do protocolo já existem para esta sessão");

            var protocolData = new SessionProtocolData
            {
                SessionId = sessionId,
                TotalDuration = request.TotalDuration,
                Notes = request.Notes
            };

            _context.SessionProtocolData.Add(protocolData);
            await _context.SaveChangesAsync();

            // Adicionar records e intervals
            foreach (var recordRequest in request.Records)
            {
                var record = new ProtocolRecord
                {
                    ProtocolDataId = protocolData.ProtocolDataId,
                    Type = recordRequest.Type,
                    Name = recordRequest.Name,
                    Order = recordRequest.Order
                };

                _context.ProtocolRecords.Add(record);
                await _context.SaveChangesAsync();

                foreach (var intervalRequest in recordRequest.Intervals)
                {
                    var interval = new TimeInterval
                    {
                        RecordId = record.RecordId,
                        Minutes = intervalRequest.Minutes,
                        Correct = intervalRequest.Correct,
                        Incorrect = intervalRequest.Incorrect
                    };

                    _context.TimeIntervals.Add(interval);
                }
            }

            await _context.SaveChangesAsync();

            return await GetBySessionIdAsync(sessionId, psychologistId, "Psychologist") 
                ?? throw new InvalidOperationException("Erro ao buscar dados criados");
        }

        public async Task<SessionProtocolDataResponse?> UpdateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return null;

            var existing = await _context.SessionProtocolData
                .Include(spd => spd.Records)
                    .ThenInclude(r => r.Intervals)
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);

            if (existing == null)
                return null;

            // Atualizar dados principais
            existing.TotalDuration = request.TotalDuration;
            existing.Notes = request.Notes;
            existing.UpdatedAt = DateTime.UtcNow;

            // Remover records e intervals antigos
            foreach (var record in existing.Records.ToList())
            {
                _context.TimeIntervals.RemoveRange(record.Intervals);
                _context.ProtocolRecords.Remove(record);
            }

            await _context.SaveChangesAsync();

            // Adicionar novos records e intervals
            foreach (var recordRequest in request.Records)
            {
                var record = new ProtocolRecord
                {
                    ProtocolDataId = existing.ProtocolDataId,
                    Type = recordRequest.Type,
                    Name = recordRequest.Name,
                    Order = recordRequest.Order
                };

                _context.ProtocolRecords.Add(record);
                await _context.SaveChangesAsync();

                foreach (var intervalRequest in recordRequest.Intervals)
                {
                    var interval = new TimeInterval
                    {
                        RecordId = record.RecordId,
                        Minutes = intervalRequest.Minutes,
                        Correct = intervalRequest.Correct,
                        Incorrect = intervalRequest.Incorrect
                    };

                    _context.TimeIntervals.Add(interval);
                }
            }

            await _context.SaveChangesAsync();

            return await GetBySessionIdAsync(sessionId, psychologistId, "Psychologist");
        }

        public async Task<bool> DeleteAsync(Guid sessionId, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return false;

            var protocolData = await _context.SessionProtocolData
                .Include(spd => spd.Records)
                    .ThenInclude(r => r.Intervals)
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);

            if (protocolData == null)
                return false;

            _context.SessionProtocolData.Remove(protocolData);
            await _context.SaveChangesAsync();

            return true;
        }

        private SessionProtocolDataResponse MapToResponse(SessionProtocolData data)
        {
            return new SessionProtocolDataResponse
            {
                ProtocolDataId = data.ProtocolDataId,
                SessionId = data.SessionId,
                TotalDuration = data.TotalDuration,
                Notes = data.Notes,
                CreatedAt = data.CreatedAt,
                UpdatedAt = data.UpdatedAt,
                Records = data.Records
                    .OrderBy(r => r.Order)
                    .Select(r => new ProtocolRecordResponse
                    {
                        RecordId = r.RecordId,
                        Type = r.Type,
                        Name = r.Name,
                        Order = r.Order,
                        Intervals = r.Intervals
                            .OrderBy(i => i.Minutes)
                            .Select(i => new TimeIntervalResponse
                            {
                                Minutes = i.Minutes,
                                Correct = i.Correct,
                                Incorrect = i.Incorrect
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        private async Task<bool> CanAccessChild(Guid childId, Guid userId, string userRole)
        {
            var child = await _childRepository.GetWithDetailsAsync(childId);
            if (child == null || !child.IsActive)
                return false;

            if (userRole == "Psychologist")
            {
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
                return psychologist != null && child.AssignedPsychologistId == psychologist.PsychologistId;
            }
            else if (userRole == "Parent")
            {
                var parent = await _userRepository.GetParentByUserIdAsync(userId);
                return parent != null &&
                       (child.PrimaryParentId == parent.ParentId || child.SecondaryParentId == parent.ParentId);
            }

            return false;
        }
    }
}