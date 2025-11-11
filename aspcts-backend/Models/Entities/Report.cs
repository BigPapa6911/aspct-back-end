using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; } = Guid.NewGuid();
        
        [ForeignKey("Child")]
        public Guid ChildId { get; set; }
        public Child Child { get; set; } = null!;
        
        [ForeignKey("Psychologist")]
        public Guid PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;
        
        public DateTime ReportDate { get; set; }
        
        [StringLength(100)]
        public string ReportType { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? FilePath { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string? SummaryForParent { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string? ClinicalNotes { get; set; }
        
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        
        public bool IsSharedWithParent { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relacionamento com dados de sessão do protocolo ABA
        public ICollection<SessionProtocolData> SessionsProtocolData { get; set; } = new List<SessionProtocolData>();
    }
}
