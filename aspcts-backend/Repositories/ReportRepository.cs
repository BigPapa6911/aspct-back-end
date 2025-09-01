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
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Report>> GetByChildIdAsync(Guid childId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(r => r.ChildId == childId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetSharedWithParentsAsync(Guid childId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(r => r.ChildId == childId && r.IsSharedWithParent)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Where(r => r.PsychologistId == psychologistId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }
    }
}