using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    // Dados do protocolo ABA para cada sessão
    public class SessionProtocolData
    {
        [Key]
        public Guid SessionProtocolDataId { get; set; } = Guid.NewGuid();
        
        // Relacionamento com a sessão existente
        [ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public Session Session { get; set; } = null!;
        
        // Pode estar vinculado a um relatório (opcional)
        [ForeignKey("Report")]
        public Guid? ReportId { get; set; }
        public Report? Report { get; set; }
        
        // Linha 1: Quantidade total da sessão
        public int TotalTrials { get; set; }
        
        // Linha 2: Atenção/Foco
        public int AttentionCorrect { get; set; }
        public int AttentionTotal { get; set; }
        
        // Linha 3: Imitação
        public int ImitationCorrect { get; set; }
        public int ImitationTotal { get; set; }
        
        // Linha 4: Ao contato (Tato/Nomeação)
        public int ContactCorrect { get; set; }
        public int ContactTotal { get; set; }
        
        // Linha 5: Atividades de Mesa
        public int DeskActivitiesCorrect { get; set; }
        public int DeskActivitiesTotal { get; set; }
        
        // Linha 6: Independência/Autonomia
        public int IndependenceCorrect { get; set; }
        public int IndependenceTotal { get; set; }
        
        // Linha 7: Tempo
        public int TimeRegistered { get; set; }
        public int TimeTotal { get; set; }
        
        [StringLength(500)]
        public string? ProtocolNotes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}