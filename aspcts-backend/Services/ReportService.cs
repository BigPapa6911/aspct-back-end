using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Report;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IChildRepository _childRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            IChildRepository childRepository,
            ISessionRepository sessionRepository,
            IAssessmentRepository assessmentRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _childRepository = childRepository;
            _sessionRepository = sessionRepository;
            _assessmentRepository = assessmentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ReportResponse> CreateReportAsync(ReportRequest request, Guid psychologistId)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            var report = new Report
            {
                ChildId = request.ChildId,
                PsychologistId = psychologistId,
                StartPeriod = request.StartPeriod,
                EndPeriod = request.EndPeriod,
                ReportType = request.ReportType,
                SummaryForParent = request.SummaryForParent,
                ClinicalNotes = request.ClinicalNotes,
                IsSharedWithParent = request.IsSharedWithParent,
                ReportDate = DateTime.UtcNow
            };

            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            var createdReport = await _reportRepository.GetByChildIdAsync(request.ChildId);
            var reportDetail = createdReport.FirstOrDefault(r => r.ReportId == report.ReportId);

            var response = _mapper.Map<ReportResponse>(reportDetail);
            response.Statistics = await GenerateStatistics(request.ChildId, request.StartPeriod, request.EndPeriod);

            return response;
        }

        public async Task<ReportResponse?> GetReportByIdAsync(Guid reportId, Guid userId, string userRole)
        {
            var reports = await _reportRepository.GetAllAsync();
            var report = reports.FirstOrDefault(r => r.ReportId == reportId);

            if (report == null)
                return null;

            var canAccess = await CanAccessChild(report.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            // For parents, only return shared reports
            if (userRole == "Parent" && !report.IsSharedWithParent)
                return null;

            var reportWithDetails = await _reportRepository.GetByChildIdAsync(report.ChildId);
            var reportDetail = reportWithDetails.FirstOrDefault(r => r.ReportId == reportId);

            var response = _mapper.Map<ReportResponse>(reportDetail);
            response.Statistics = await GenerateStatistics(report.ChildId, report.StartPeriod, report.EndPeriod);

            return response;
        }

        public async Task<IEnumerable<ReportResponse>> GetReportsByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return new List<ReportResponse>();

            var reports = userRole == "Parent"
                ? await _reportRepository.GetSharedWithParentsAsync(childId)
                : await _reportRepository.GetByChildIdAsync(childId);

            var responses = new List<ReportResponse>();
            foreach (var report in reports)
            {
                var response = _mapper.Map<ReportResponse>(report);
                response.Statistics = await GenerateStatistics(childId, report.StartPeriod, report.EndPeriod);
                responses.Add(response);
            }

            return responses;
        }

        public async Task<bool> ShareWithParentAsync(Guid reportId, bool share, Guid psychologistId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.PsychologistId != psychologistId)
                return false;

            report.IsSharedWithParent = share;
            _reportRepository.Update(report);
            await _reportRepository.SaveChangesAsync();

            return true;
        }

        public async Task<byte[]> GeneratePdfReportAsync(Guid reportId, Guid userId, string userRole)
        {
            // This would integrate with a PDF generation library like iTextSharp
            // For now, returning empty byte array
            var report = await GetReportByIdAsync(reportId, userId, userRole);
            if (report == null)
                throw new ArgumentException("Relatório não encontrado ou acesso negado");

            // TODO: Implement PDF generation
            return Array.Empty<byte>();
        }

        private async Task<ReportStatistics> GenerateStatistics(Guid childId, DateTime startPeriod, DateTime endPeriod)
        {
            var sessions = await _sessionRepository.GetSessionsByDateRangeAsync(childId, startPeriod, endPeriod);
            var assessments = await _assessmentRepository.GetByChildIdAsync(childId);
            var periodAssessments = assessments.Where(a => a.AssessmentDate >= startPeriod && a.AssessmentDate <= endPeriod);

            return new ReportStatistics
            {
                TotalSessions = sessions.Count(),
                TotalAssessments = periodAssessments.Count(),
                AssessmentsByType = periodAssessments.GroupBy(a => a.AssessmentType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                SessionsByType = sessions.GroupBy(s => s.SessionType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AchievedGoals = new List<string>(), // TODO: Implement based on intervention plans
                ActiveGoals = new List<string>() // TODO: Implement based on intervention plans
            };
        }

        private async Task<bool> CanAccessChild(Guid childId, Guid userId, string userRole)
        {
            var child = await _childRepository.GetWithDetailsAsync(childId);
            if (child == null || !child.IsActive)
                return false;

            if (userRole == "Psychologist")
            {
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
                return psychologist != null && child.AssignedPsychologistId == psychologist.PsychologistId;
            }
            else if (userRole == "Parent")
            {
                var parent = await _userRepository.GetParentByUserIdAsync(userId);
                return parent != null &&
                       (child.PrimaryParentId == parent.ParentId || child.SecondaryParentId == parent.ParentId);
            }

            return false;
        }
    }
}