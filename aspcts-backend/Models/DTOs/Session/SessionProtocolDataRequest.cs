using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionProtocolDataRequest
    {
        [Required]
        [Range(1, 480)]
        public int TotalDuration { get; set; }  // Duração da sessão em minutos
        
        [Required]
        public List<ProtocolRecordRequest> Records { get; set; } = new();
        
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class ProtocolRecordRequest
    {
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;  // "behavior", "demand", "event"
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;  // Ex: "Demanda 1°", "Comportamento problema 1°"
        
        [Required]
        public int Order { get; set; }  // Ordem de exibição
        
        [Required]
        public List<TimeIntervalRequest> Intervals { get; set; } = new();
    }

    public class TimeIntervalRequest
    {
        [Required]
        [Range(5, 60)]
        public int Minutes { get; set; }  // 5, 10, 15, 20, ..., 60
        
        [Range(0, int.MaxValue)]
        public int Correct { get; set; }  // Acertos neste intervalo
        
        [Range(0, int.MaxValue)]
        public int Incorrect { get; set; }  // Erros neste intervalo
    }
}