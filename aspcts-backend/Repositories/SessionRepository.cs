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
    public class SessionRepository : GenericRepository<Session>, ISessionRepository

    {
        public SessionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Session>> GetByChildIdAsync(Guid childId)
        {
            return await _context.Sessions
                .Include(s => s.Child)
                .Include(s => s.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(s => s.ChildId == childId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.Sessions
                .Include(s => s.Child)
                .Where(s => s.PsychologistId == psychologistId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetSharedWithParentsAsync(Guid childId)
        {
            return await _context.Sessions
                .Include(s => s.Child)
                .Include(s => s.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(s => s.ChildId == childId && s.IsSharedWithParent)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetSessionsByDateRangeAsync(Guid childId, DateTime startDate, DateTime endDate)
        {
            return await _context.Sessions
                .Include(s => s.Child)
                .Include(s => s.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(s => s.ChildId == childId && s.SessionDate >= startDate && s.SessionDate <= endDate)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }
    }
}