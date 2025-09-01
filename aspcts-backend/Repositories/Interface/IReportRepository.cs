using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<IEnumerable<Report>> GetByChildIdAsync(Guid childId);
        Task<IEnumerable<Report>> GetSharedWithParentsAsync(Guid childId);
        Task<IEnumerable<Report>> GetByPsychologistIdAsync(Guid psychologistId);
    }
}