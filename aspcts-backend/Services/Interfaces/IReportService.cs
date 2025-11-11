using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Report;

namespace aspcts_backend.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportResponse> CreateReportAsync(ReportRequest request, Guid psychologistId);
        Task<ReportResponse?> GetReportByIdAsync(Guid reportId, Guid userId, string userRole);
        Task<IEnumerable<ReportResponse>> GetReportsByChildIdAsync(Guid childId, Guid userId, string userRole);
        Task<bool> ShareWithParentAsync(Guid reportId, bool share, Guid psychologistId);
        Task<byte[]> GeneratePdfReportAsync(Guid reportId, Guid userId, string userRole);
        Task<bool> AddSessionsToReportAsync(Guid reportId, List<Guid> sessionIds, Guid psychologistId);
        Task<bool> RemoveSessionFromReportAsync(Guid reportId, Guid sessionId, Guid psychologistId);
    }
}