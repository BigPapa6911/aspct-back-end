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
    public class ChildRepository : GenericRepository<Child>, IChildRepository
    {
        public ChildRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Child>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.Children
                .Include(c => c.PrimaryParent)
                    .ThenInclude(p => p.User)
                .Include(c => c.SecondaryParent)
                    .ThenInclude(p => p.User)
                .Where(c => c.AssignedPsychologistId == psychologistId && c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Child>> GetByParentIdAsync(Guid parentId)
        {
            return await _context.Children
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .Include(c => c.PrimaryParent)
                    .ThenInclude(p => p.User)
                .Include(c => c.SecondaryParent)
                    .ThenInclude(p => p.User)
                .Where(c => (c.PrimaryParentId == parentId || c.SecondaryParentId == parentId) && c.IsActive)
                .ToListAsync();
        }

        public async Task<Child?> GetWithDetailsAsync(Guid childId)
        {
            return await _context.Children
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .Include(c => c.PrimaryParent)
                    .ThenInclude(p => p.User)
                .Include(c => c.SecondaryParent)
                    .ThenInclude(p => p.User)
                .Include(c => c.Sessions)
                .Include(c => c.Assessments)
                .FirstOrDefaultAsync(c => c.ChildId == childId);
        }

        public async Task<IEnumerable<Child>> GetActiveChildrenAsync()
        {
            return await _context.Children
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .Include(c => c.PrimaryParent)
                    .ThenInclude(p => p.User)
                .Include(c => c.SecondaryParent)
                    .ThenInclude(p => p.User)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

    }
}