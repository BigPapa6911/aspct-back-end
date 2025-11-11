using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Report
{
    public class ReportRequest
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public DateTime StartPeriod { get; set; }

        [Required]
        public DateTime EndPeriod { get; set; }

        [Required]
        [StringLength(100)]
        public string ReportType { get; set; } = string.Empty;

        public string? SummaryForParent { get; set; }

        public string? ClinicalNotes { get; set; }

        public bool IsSharedWithParent { get; set; } = false;

        // IDs das sessões que devem ser incluídas no relatório
        public List<Guid> SessionIds { get; set; } = new();
    }

    public class UpdateReportRequest
    {
        [StringLength(100)]
        public string? ReportType { get; set; }

        public string? SummaryForParent { get; set; }

        public string? ClinicalNotes { get; set; }

        public bool? IsSharedWithParent { get; set; }

        public DateTime? StartPeriod { get; set; }

        public DateTime? EndPeriod { get; set; }
    }
}