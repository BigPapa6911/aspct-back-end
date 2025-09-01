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
    public class AssessmentRepository : GenericRepository<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAssessmentsByTypeAsync<T>(Guid childId) where T : Assessment
        {
            return await _context.Set<T>()
                .Include(a => a.Child)
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.ChildId == childId)
                .OrderByDescending(a => a.AssessmentDate)
                .ToListAsync();
        }

        public async Task<T?> GetLatestAssessmentByTypeAsync<T>(Guid childId) where T : Assessment
        {
            return await _context.Set<T>()
                .Include(a => a.Child)
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.ChildId == childId)
                .OrderByDescending(a => a.AssessmentDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Assessment>> GetByChildIdAsync(Guid childId)
        {
            return await _context.Set<Assessment>()
                .Include(a => a.Child)
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.ChildId == childId)
                .OrderByDescending(a => a.AssessmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assessment>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.Set<Assessment>()
                .Include(a => a.Child)
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.PsychologistId == psychologistId)
                .OrderByDescending(a => a.AssessmentDate)
                .ToListAsync();
        }
    }
}