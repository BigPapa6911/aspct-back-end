using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class ProtocolRecord
    {
        [Key]
        public Guid RecordId { get; set; } = Guid.NewGuid();
        
        [ForeignKey("ProtocolData")]
        public Guid ProtocolDataId { get; set; }
        public SessionProtocolData ProtocolData { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // "behavior", "demand", "event"
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty; // Ex: "Demanda 1°", "Comportamento problema 1°"
        
        public int Order { get; set; } // Ordem de exibição
        
        public ICollection<TimeInterval> Intervals { get; set; } = new List<TimeInterval>();
    }
}