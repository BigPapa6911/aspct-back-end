using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface IInterventionPlanRepository : IGenericRepository<InterventionPlan>
    {
        Task<IEnumerable<InterventionPlan>> GetByChildIdAsync(Guid childId);
        Task<InterventionPlan?> GetActiveByChildIdAsync(Guid childId);
        Task<InterventionPlan?> GetWithGoalsAsync(Guid planId);
    }
}