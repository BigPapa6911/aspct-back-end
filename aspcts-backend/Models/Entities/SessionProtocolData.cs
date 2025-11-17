using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class SessionProtocolData
    {
        [Key]
        public Guid ProtocolDataId { get; set; } = Guid.NewGuid();
        
        [ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public Session Session { get; set; } = null!;
        
        [ForeignKey("Report")]
        public Guid? ReportId { get; set; }
        public Report? Report { get; set; }
        
        public int TotalDuration { get; set; } // Duração em minutos
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<ProtocolRecord> Records { get; set; } = new List<ProtocolRecord>();
    }
}