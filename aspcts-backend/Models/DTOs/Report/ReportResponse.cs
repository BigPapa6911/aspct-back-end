using System;
using System.Collections.Generic;
using aspcts_backend.Models.DTOs.Common;

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

        public List<SessionWithProtocolResponse> Sessions { get; set; } = new();
        public ReportStatistics Statistics { get; set; } = new();
    }

    public class SessionWithProtocolResponse
    {
        public Guid SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public int Duration { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public string? NotesWhatWasDone { get; set; }
        
        public Guid? ProtocolDataId { get; set; }
        public int TotalTrials { get; set; }
        
        public MetricData Attention { get; set; } = new();
        public MetricData Imitation { get; set; } = new();
        public MetricData Contact { get; set; } = new();
        public MetricData DeskActivities { get; set; } = new();
        public MetricData Independence { get; set; } = new();
        public MetricData Time { get; set; } = new();
        
        public string? ProtocolNotes { get; set; }
    }

    public class ReportStatistics
    {
        public int TotalSessions { get; set; }
        public int TotalTrials { get; set; }
        public int TotalMinutes { get; set; }
        
        public SkillAreaStats Attention { get; set; } = new();
        public SkillAreaStats Imitation { get; set; } = new();
        public SkillAreaStats Contact { get; set; } = new();
        public SkillAreaStats DeskActivities { get; set; } = new();
        public SkillAreaStats Independence { get; set; } = new();
        public SkillAreaStats Time { get; set; } = new();
        
        public decimal OverallProgress { get; set; }
        public List<string> Highlights { get; set; } = new();
        public List<string> AreasForImprovement { get; set; } = new();
    }

    public class SkillAreaStats
    {
        public int TotalCorrect { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AveragePercentage => TotalAttempts > 0 ? Math.Round((decimal)TotalCorrect / TotalAttempts * 100, 1) : 0;
        public int SessionsWorked { get; set; }
        public decimal Trend { get; set; }
    }
}