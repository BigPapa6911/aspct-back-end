using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface IChildRepository : IGenericRepository<Child>
    {
        Task<IEnumerable<Child>> GetByPsychologistIdAsync(Guid psychologistId);
        Task<IEnumerable<Child>> GetByParentIdAsync(Guid parentId);
        Task<Child?> GetWithDetailsAsync(Guid childId);
        Task<IEnumerable<Child>> GetActiveChildrenAsync();
    }
}