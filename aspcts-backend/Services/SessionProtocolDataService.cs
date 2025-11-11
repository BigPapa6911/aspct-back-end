using System;
using System.Threading.Tasks;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Models.DTOs.Common;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class SessionProtocolDataService : ISessionProtocolDataService
    {
        private readonly ISessionProtocolDataRepository _protocolDataRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;

        public SessionProtocolDataService(
            ISessionProtocolDataRepository protocolDataRepository,
            ISessionRepository sessionRepository,
            IChildRepository childRepository,
            IUserRepository userRepository)
        {
            _protocolDataRepository = protocolDataRepository;
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

            var data = await _protocolDataRepository.GetBySessionIdAsync(sessionId);
            if (data == null)
                return null;

            return MapToResponse(data);
        }

        public async Task<SessionProtocolDataResponse> CreateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                throw new ArgumentException("Sessão não encontrada ou acesso negado");

            var existing = await _protocolDataRepository.GetBySessionIdAsync(sessionId);
            if (existing != null)
                throw new InvalidOperationException("Dados do protocolo já existem para esta sessão");

            var protocolData = new SessionProtocolData
            {
                SessionId = sessionId,
                TotalTrials = request.TotalTrials,
                AttentionCorrect = request.AttentionCorrect,
                AttentionTotal = request.AttentionTotal,
                ImitationCorrect = request.ImitationCorrect,
                ImitationTotal = request.ImitationTotal,
                ContactCorrect = request.ContactCorrect,
                ContactTotal = request.ContactTotal,
                DeskActivitiesCorrect = request.DeskActivitiesCorrect,
                DeskActivitiesTotal = request.DeskActivitiesTotal,
                IndependenceCorrect = request.IndependenceCorrect,
                IndependenceTotal = request.IndependenceTotal,
                TimeRegistered = request.TimeRegistered,
                TimeTotal = request.TimeTotal,
                ProtocolNotes = request.ProtocolNotes
            };

            await _protocolDataRepository.AddAsync(protocolData);
            await _protocolDataRepository.SaveChangesAsync();

            return MapToResponse(protocolData);
        }

        public async Task<SessionProtocolDataResponse?> UpdateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return null;

            var existing = await _protocolDataRepository.GetBySessionIdAsync(sessionId);
            if (existing == null)
                return null;

            existing.TotalTrials = request.TotalTrials;
            existing.AttentionCorrect = request.AttentionCorrect;
            existing.AttentionTotal = request.AttentionTotal;
            existing.ImitationCorrect = request.ImitationCorrect;
            existing.ImitationTotal = request.ImitationTotal;
            existing.ContactCorrect = request.ContactCorrect;
            existing.ContactTotal = request.ContactTotal;
            existing.DeskActivitiesCorrect = request.DeskActivitiesCorrect;
            existing.DeskActivitiesTotal = request.DeskActivitiesTotal;
            existing.IndependenceCorrect = request.IndependenceCorrect;
            existing.IndependenceTotal = request.IndependenceTotal;
            existing.TimeRegistered = request.TimeRegistered;
            existing.TimeTotal = request.TimeTotal;
            existing.ProtocolNotes = request.ProtocolNotes;
            existing.UpdatedAt = DateTime.UtcNow;

            _protocolDataRepository.Update(existing);
            await _protocolDataRepository.SaveChangesAsync();

            return MapToResponse(existing);
        }

        public async Task<bool> DeleteAsync(Guid sessionId, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return false;

            return await _protocolDataRepository.DeleteBySessionIdAsync(sessionId);
        }

        private SessionProtocolDataResponse MapToResponse(SessionProtocolData data)
        {
            return new SessionProtocolDataResponse
            {
                ProtocolDataId = data.SessionProtocolDataId,
                TotalTrials = data.TotalTrials,
                Attention = new MetricData 
                { 
                    Correct = data.AttentionCorrect, 
                    Total = data.AttentionTotal 
                },
                Imitation = new MetricData 
                { 
                    Correct = data.ImitationCorrect, 
                    Total = data.ImitationTotal 
                },
                Contact = new MetricData 
                { 
                    Correct = data.ContactCorrect, 
                    Total = data.ContactTotal 
                },
                DeskActivities = new MetricData 
                { 
                    Correct = data.DeskActivitiesCorrect, 
                    Total = data.DeskActivitiesTotal 
                },
                Independence = new MetricData 
                { 
                    Correct = data.IndependenceCorrect, 
                    Total = data.IndependenceTotal 
                },
                Time = new MetricData 
                { 
                    Correct = data.TimeRegistered, 
                    Total = data.TimeTotal 
                },
                ProtocolNotes = data.ProtocolNotes
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