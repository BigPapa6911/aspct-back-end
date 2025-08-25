using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Report
{
    public class ReportResponse
    {
        public Guid ReportId { get; set; }
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public Guid PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public string? SummaryForParent { get; set; }
        public string? ClinicalNotes { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public bool IsSharedWithParent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Statistics for the report period
        public ReportStatistics Statistics { get; set; } = new();
    }
    public class ReportStatistics
    {
        public int TotalSessions { get; set; }
        public int TotalAssessments { get; set; }
        public Dictionary<string, int> AssessmentsByType { get; set; } = new();
        public Dictionary<string, int> SessionsByType { get; set; } = new();
        public List<string> AchievedGoals { get; set; } = new();
        public List<string> ActiveGoals { get; set; } = new();
    }
}