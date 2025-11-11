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
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
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
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
                .Where(r => r.ChildId == childId && r.IsSharedWithParent)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
                .Where(r => r.PsychologistId == psychologistId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdWithSessionsAsync(Guid reportId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);
        }

        public async Task<IEnumerable<Report>> GetByChildIdWithSessionsAsync(Guid childId)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
                .Where(r => r.ChildId == childId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByPeriodAsync(Guid childId, DateTime startDate, DateTime endDate)
        {
            return await _context.Reports
                .Include(r => r.Child)
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.SessionsProtocolData)
                    .ThenInclude(spd => spd.Session)
                .Where(r => r.ChildId == childId && 
                           r.StartPeriod >= startDate && 
                           r.EndPeriod <= endDate)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<bool> AddSessionsToReportAsync(Guid reportId, List<Guid> sessionIds)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return false;

            foreach (var sessionId in sessionIds)
            {
                var protocolData = await _context.SessionProtocolData
                    .FirstOrDefaultAsync(spd => spd.SessionId == sessionId);

                if (protocolData != null)
                {
                    protocolData.ReportId = reportId;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSessionFromReportAsync(Guid reportId, Guid sessionId)
        {
            var protocolData = await _context.SessionProtocolData
                .FirstOrDefaultAsync(spd => spd.SessionId == sessionId && spd.ReportId == reportId);

            if (protocolData == null)
                return false;

            protocolData.ReportId = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
