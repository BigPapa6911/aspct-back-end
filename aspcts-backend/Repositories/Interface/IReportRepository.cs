using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<IEnumerable<Report>> GetByChildIdAsync(Guid childId);
        Task<IEnumerable<Report>> GetSharedWithParentsAsync(Guid childId);
        Task<IEnumerable<Report>> GetByPsychologistIdAsync(Guid psychologistId);
        Task<Report?> GetByIdWithSessionsAsync(Guid reportId);
        Task<IEnumerable<Report>> GetByChildIdWithSessionsAsync(Guid childId);
        Task<IEnumerable<Report>> GetByPeriodAsync(Guid childId, DateTime startDate, DateTime endDate);
        Task<bool> AddSessionsToReportAsync(Guid reportId, List<Guid> sessionIds);
        Task<bool> RemoveSessionFromReportAsync(Guid reportId, Guid sessionId);
    }
}
