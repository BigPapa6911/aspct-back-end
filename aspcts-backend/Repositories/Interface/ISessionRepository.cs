using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetByChildIdAsync(Guid childId);
        Task<IEnumerable<Session>> GetByPsychologistIdAsync(Guid psychologistId);
        Task<IEnumerable<Session>> GetSharedWithParentsAsync(Guid childId);
        Task<IEnumerable<Session>> GetSessionsByDateRangeAsync(Guid childId, DateTime startDate, DateTime endDate);
    }
}