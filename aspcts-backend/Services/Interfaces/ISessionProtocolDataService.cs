using System;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Session;
using static aspcts_backend.Models.DTOs.Session.SessionResponse;

namespace aspcts_backend.Services.Interfaces
{
    public interface ISessionProtocolDataService
    {
        Task<SessionProtocolDataResponse?> GetBySessionIdAsync(Guid sessionId, Guid userId, string userRole);
        Task<SessionProtocolDataResponse> CreateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId);
        Task<SessionProtocolDataResponse?> UpdateAsync(Guid sessionId, SessionProtocolDataRequest request, Guid psychologistId);
        Task<bool> DeleteAsync(Guid sessionId, Guid psychologistId);
    }
}