using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Session;

namespace aspcts_backend.Services.Interfaces
{
    public interface ISessionService
    {
        Task<SessionResponse> CreateSessionAsync(SessionCreateRequest request, Guid psychologistId);
        Task<SessionResponse?> GetSessionByIdAsync(Guid sessionId, Guid userId, string userRole);
        Task<IEnumerable<SessionResponse>> GetSessionsByChildIdAsync(Guid childId, Guid userId, string userRole);
        Task<SessionResponse?> UpdateSessionAsync(Guid sessionId, SessionUpdateRequest request, Guid psychologistId);
        Task<bool> DeleteSessionAsync(Guid sessionId, Guid psychologistId);
        Task<bool> ShareWithParentAsync(Guid sessionId, bool share, Guid psychologistId);
    }
}