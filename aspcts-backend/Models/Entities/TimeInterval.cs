using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class TimeInterval
    {
        [Key]
        public Guid IntervalId { get; set; } = Guid.NewGuid();
        
        [ForeignKey("Record")]
        public Guid RecordId { get; set; }
        public ProtocolRecord Record { get; set; } = null!;
        
        [Range(5, 60)]
        public int Minutes { get; set; } // 5, 10, 15, 20, ..., 60
        
        public int Correct { get; set; } // Acertos neste intervalo
        public int Incorrect { get; set; } // Erros neste intervalo
    }
}