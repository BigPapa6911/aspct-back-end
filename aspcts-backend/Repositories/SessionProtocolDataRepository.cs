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
    public class SessionProtocolDataRepository : GenericRepository<SessionProtocolData>, ISessionProtocolDataRepository
    {
        public SessionProtocolDataRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SessionProtocolData?> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.SessionProtocolData
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Child)
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Psychologist)
                .Include(spd => spd.Report)
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);
        }

        public async Task<IEnumerable<SessionProtocolData>> GetByReportIdAsync(Guid reportId)
        {
            return await _context.SessionProtocolData
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Child)
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Psychologist)
                .Where(spd => spd.ReportId == reportId)
                .OrderBy(spd => spd.Session.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionProtocolData>> GetByChildIdAsync(Guid childId)
        {
            return await _context.SessionProtocolData
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Child)
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Psychologist)
                .Include(spd => spd.Report)
                .Where(spd => spd.Session.ChildId == childId)
                .OrderByDescending(spd => spd.Session.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionProtocolData>> GetByDateRangeAsync(Guid childId, DateTime startDate, DateTime endDate)
        {
            return await _context.SessionProtocolData
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Child)
                .Include(spd => spd.Session)
                    .ThenInclude(s => s.Psychologist)
                .Include(spd => spd.Report)
                .Where(spd => spd.Session.ChildId == childId && 
                             spd.Session.SessionDate >= startDate && 
                             spd.Session.SessionDate <= endDate)
                .OrderBy(spd => spd.Session.SessionDate)
                .ToListAsync();
        }

        public async Task<bool> DeleteBySessionIdAsync(Guid sessionId)
        {
            var protocolData = await _context.SessionProtocolData
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);
            
            if (protocolData == null)
                return false;

            _context.SessionProtocolData.Remove(protocolData);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
