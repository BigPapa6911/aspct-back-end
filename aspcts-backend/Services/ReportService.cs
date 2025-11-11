using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Report;
using aspcts_backend.Models.Entities;
using aspcts_backend.Models.DTOs.Common;

namespace aspcts_backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IChildRepository _childRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionProtocolDataRepository _protocolDataRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            IChildRepository childRepository,
            ISessionRepository sessionRepository,
            ISessionProtocolDataRepository protocolDataRepository,
            IAssessmentRepository assessmentRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _childRepository = childRepository;
            _sessionRepository = sessionRepository;
            _protocolDataRepository = protocolDataRepository;
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

            // Vincular sessões ao relatório se fornecidas
            if (request.SessionIds != null && request.SessionIds.Any())
            {
                await _reportRepository.AddSessionsToReportAsync(report.ReportId, request.SessionIds);
            }

            // Buscar relatório com todos os dados
            var createdReport = await _reportRepository.GetByIdWithSessionsAsync(report.ReportId);

            return await MapToDetailedResponse(createdReport!);
        }

        public async Task<ReportResponse?> GetReportByIdAsync(Guid reportId, Guid userId, string userRole)
        {
            var report = await _reportRepository.GetByIdWithSessionsAsync(reportId);

            if (report == null)
                return null;

            var canAccess = await CanAccessChild(report.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            // For parents, only return shared reports
            if (userRole == "Parent" && !report.IsSharedWithParent)
                return null;

            return await MapToDetailedResponse(report);
        }

        public async Task<IEnumerable<ReportResponse>> GetReportsByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return new List<ReportResponse>();

            var reports = userRole == "Parent"
                ? await _reportRepository.GetSharedWithParentsAsync(childId)
                : await _reportRepository.GetByChildIdWithSessionsAsync(childId);

            var responses = new List<ReportResponse>();
            foreach (var report in reports)
            {
                var response = await MapToDetailedResponse(report);
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
            report.UpdatedAt = DateTime.UtcNow;

            _reportRepository.Update(report);
            await _reportRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddSessionsToReportAsync(Guid reportId, List<Guid> sessionIds, Guid psychologistId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.PsychologistId != psychologistId)
                return false;

            return await _reportRepository.AddSessionsToReportAsync(reportId, sessionIds);
        }

        public async Task<bool> RemoveSessionFromReportAsync(Guid reportId, Guid sessionId, Guid psychologistId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.PsychologistId != psychologistId)
                return false;

            return await _reportRepository.RemoveSessionFromReportAsync(reportId, sessionId);
        }

        public async Task<byte[]> GeneratePdfReportAsync(Guid reportId, Guid userId, string userRole)
        {
            var report = await GetReportByIdAsync(reportId, userId, userRole);
            if (report == null)
                throw new ArgumentException("Relatório não encontrado ou acesso negado");

            // TODO: Implement PDF generation with iTextSharp or similar
            return Array.Empty<byte>();
        }

        private async Task<ReportResponse> MapToDetailedResponse(Report report)
        {
            var response = _mapper.Map<ReportResponse>(report);

            // Mapear sessões com dados do protocolo
            response.Sessions = report.SessionsProtocolData
                .Select(spd => new SessionWithProtocolResponse
                {
                    SessionId = spd.SessionId,
                    SessionDate = spd.Session.SessionDate,
                    Duration = spd.Session.Duration,
                    SessionType = spd.Session.SessionType,
                    NotesWhatWasDone = spd.Session.NotesWhatWasDone,
                    ProtocolDataId = spd.SessionProtocolDataId,
                    TotalTrials = spd.TotalTrials,
                    Attention = new MetricData { Correct = spd.AttentionCorrect, Total = spd.AttentionTotal },
                    Imitation = new MetricData { Correct = spd.ImitationCorrect, Total = spd.ImitationTotal },
                    Contact = new MetricData { Correct = spd.ContactCorrect, Total = spd.ContactTotal },
                    DeskActivities = new MetricData { Correct = spd.DeskActivitiesCorrect, Total = spd.DeskActivitiesTotal },
                    Independence = new MetricData { Correct = spd.IndependenceCorrect, Total = spd.IndependenceTotal },
                    Time = new MetricData { Correct = spd.TimeRegistered, Total = spd.TimeTotal },
                    ProtocolNotes = spd.ProtocolNotes
                })
                .OrderBy(s => s.SessionDate)
                .ToList();

            // Gerar estatísticas
            response.Statistics = GenerateStatistics(response.Sessions);

            return response;
        }

        private ReportStatistics GenerateStatistics(List<SessionWithProtocolResponse> sessions)
        {
            if (!sessions.Any())
                return new ReportStatistics();

            var stats = new ReportStatistics
            {
                TotalSessions = sessions.Count,
                TotalTrials = sessions.Sum(s => s.TotalTrials),
                TotalMinutes = sessions.Sum(s => s.Duration),
                Attention = CalculateSkillAreaStats(sessions.Select(s => s.Attention).ToList()),
                Imitation = CalculateSkillAreaStats(sessions.Select(s => s.Imitation).ToList()),
                Contact = CalculateSkillAreaStats(sessions.Select(s => s.Contact).ToList()),
                DeskActivities = CalculateSkillAreaStats(sessions.Select(s => s.DeskActivities).ToList()),
                Independence = CalculateSkillAreaStats(sessions.Select(s => s.Independence).ToList()),
                Time = CalculateSkillAreaStats(sessions.Select(s => s.Time).ToList())
            };

            // Calcular progresso geral
            var allAreas = new[] { stats.Attention, stats.Imitation, stats.Contact, stats.DeskActivities, stats.Independence };
            stats.OverallProgress = allAreas.Average(a => a.AveragePercentage);

            // Gerar highlights e áreas de melhoria
            stats.Highlights = GenerateHighlights(stats);
            stats.AreasForImprovement = GenerateAreasForImprovement(stats);

            return stats;
        }

        private SkillAreaStats CalculateSkillAreaStats(List<MetricData> metrics)
        {
            var metricsWithData = metrics.Where(m => m.Total > 0).ToList();

            return new SkillAreaStats
            {
                TotalCorrect = metrics.Sum(m => m.Correct),
                TotalAttempts = metrics.Sum(m => m.Total),
                SessionsWorked = metricsWithData.Count,
                Trend = CalculateTrend(metricsWithData)
            };
        }

        private decimal CalculateTrend(List<MetricData> metrics)
        {
            if (metrics.Count < 2)
                return 0;

            var firstHalf = metrics.Take(metrics.Count / 2).ToList();
            var secondHalf = metrics.Skip(metrics.Count / 2).ToList();

            var firstAvg = firstHalf.Any() ? firstHalf.Average(m => m.Percentage) : 0;
            var secondAvg = secondHalf.Any() ? secondHalf.Average(m => m.Percentage) : 0;

            return secondAvg - firstAvg;
        }

        private List<string> GenerateHighlights(ReportStatistics stats)
        {
            var highlights = new List<string>();

            var areas = new Dictionary<string, SkillAreaStats>
            {
                { "Atenção", stats.Attention },
                { "Imitação", stats.Imitation },
                { "Contato", stats.Contact },
                { "Atividades de Mesa", stats.DeskActivities },
                { "Independência", stats.Independence }
            };

            foreach (var area in areas.OrderByDescending(a => a.Value.AveragePercentage).Take(3))
            {
                if (area.Value.AveragePercentage >= 80)
                {
                    highlights.Add($"{area.Key}: Excelente desempenho com {area.Value.AveragePercentage:F1}% de acertos");
                }
            }

            foreach (var area in areas.Where(a => a.Value.Trend > 10))
            {
                highlights.Add($"Melhora significativa em {area.Key} (+{area.Value.Trend:F1}%)");
            }

            return highlights;
        }

        private List<string> GenerateAreasForImprovement(ReportStatistics stats)
        {
            var improvements = new List<string>();

            var areas = new Dictionary<string, SkillAreaStats>
            {
                { "Atenção", stats.Attention },
                { "Imitação", stats.Imitation },
                { "Contato", stats.Contact },
                { "Atividades de Mesa", stats.DeskActivities },
                { "Independência", stats.Independence }
            };

            foreach (var area in areas.OrderBy(a => a.Value.AveragePercentage).Take(2))
            {
                if (area.Value.AveragePercentage < 70)
                {
                    improvements.Add($"{area.Key}: Necessita mais prática ({area.Value.AveragePercentage:F1}% de acertos)");
                }
            }

            foreach (var area in areas.Where(a => a.Value.Trend < -10))
            {
                improvements.Add($"{area.Key}: Atenção ao declínio recente ({area.Value.Trend:F1}%)");
            }

            return improvements;
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