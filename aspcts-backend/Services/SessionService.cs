using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SessionService(
            ISessionRepository sessionRepository,
            IChildRepository childRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _childRepository = childRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<SessionResponse> CreateSessionAsync(SessionCreateRequest request, Guid psychologistId)
        {
            // Validate child exists and belongs to psychologist
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            var session = _mapper.Map<Session>(request);
            session.PsychologistId = psychologistId;

            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveChangesAsync();

            // Fetch with related data
            var createdSession = await _sessionRepository.GetByChildIdAsync(request.ChildId);
            var sessionWithDetails = createdSession.FirstOrDefault(s => s.SessionId == session.SessionId);

            return _mapper.Map<SessionResponse>(sessionWithDetails);
        }

        public async Task<SessionResponse?> GetSessionByIdAsync(Guid sessionId, Guid userId, string userRole)
        {
            var sessions = await _sessionRepository.GetAllAsync();
            var session = sessions.FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null)
                return null;

            // Check access permissions
            var canAccess = await CanAccessSession(session.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            // For parents, only return shared sessions
            if (userRole == "Parent" && !session.IsSharedWithParent)
                return null;

            var sessionWithDetails = await _sessionRepository.GetByChildIdAsync(session.ChildId);
            var sessionDetail = sessionWithDetails.FirstOrDefault(s => s.SessionId == sessionId);

            return _mapper.Map<SessionResponse>(sessionDetail);
        }

        public async Task<IEnumerable<SessionResponse>> GetSessionsByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            // Check access permissions
            var canAccess = await CanAccessSession(childId, userId, userRole);
            if (!canAccess)
                return new List<SessionResponse>();

            var sessions = await _sessionRepository.GetByChildIdAsync(childId);

            // For parents, only return shared sessions
            if (userRole == "Parent")
            {
                sessions = sessions.Where(s => s.IsSharedWithParent);
            }

            return sessions.Select(s => _mapper.Map<SessionResponse>(s)).ToList();
        }

        public async Task<SessionResponse?> UpdateSessionAsync(Guid sessionId, SessionUpdateRequest request, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return null;

            // Update only provided fields
            _mapper.Map(request, session);
            session.UpdatedAt = DateTime.UtcNow;

            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();

            // Fetch updated session with details
            var sessions = await _sessionRepository.GetByChildIdAsync(session.ChildId);
            var updatedSession = sessions.FirstOrDefault(s => s.SessionId == sessionId);

            return _mapper.Map<SessionResponse>(updatedSession);
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return false;

            _sessionRepository.Remove(session);
            await _sessionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ShareWithParentAsync(Guid sessionId, bool share, Guid psychologistId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.PsychologistId != psychologistId)
                return false;

            session.IsSharedWithParent = share;
            session.UpdatedAt = DateTime.UtcNow;

            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();

            return true;
        }

        private async Task<bool> CanAccessSession(Guid childId, Guid userId, string userRole)
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