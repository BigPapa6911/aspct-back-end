using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface ISessionProtocolDataRepository : IGenericRepository<SessionProtocolData>
    {
        Task<SessionProtocolData?> GetBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<SessionProtocolData>> GetByReportIdAsync(Guid reportId);
        Task<IEnumerable<SessionProtocolData>> GetByChildIdAsync(Guid childId);
        Task<IEnumerable<SessionProtocolData>> GetByDateRangeAsync(Guid childId, DateTime startDate, DateTime endDate);
        Task<bool> DeleteBySessionIdAsync(Guid sessionId);
    }
}
