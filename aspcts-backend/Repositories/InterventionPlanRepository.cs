using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Data;
using aspcts_backend.Models.Entities;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Repositories
{
    public class InterventionPlanRepository : GenericRepository<InterventionPlan>, IInterventionPlanRepository
    {
        public InterventionPlanRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<InterventionPlan>> GetByChildIdAsync(Guid childId)
        {
            return await _context.InterventionPlans
                .Include(ip => ip.Child)
                .Include(ip => ip.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(ip => ip.InterventionGoals)
                .Where(ip => ip.ChildId == childId)
                .OrderByDescending(ip => ip.CreatedAt)
                .ToListAsync();
        }

        public async Task<InterventionPlan?> GetActiveByChildIdAsync(Guid childId)
        {
            return await _context.InterventionPlans
                .Include(ip => ip.Child)
                .Include(ip => ip.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(ip => ip.InterventionGoals)
                .FirstOrDefaultAsync(ip => ip.ChildId == childId && ip.Status == "Active");
        }

        public async Task<InterventionPlan?> GetWithGoalsAsync(Guid planId)
        {
            return await _context.InterventionPlans
                .Include(ip => ip.Child)
                .Include(ip => ip.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(ip => ip.InterventionGoals)
                .FirstOrDefaultAsync(ip => ip.PlanId == planId);
        }
    }
}