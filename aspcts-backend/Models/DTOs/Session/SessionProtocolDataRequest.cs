using System;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionProtocolDataRequest
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int TotalTrials { get; set; }

        [Range(0, int.MaxValue)]
        public int AttentionCorrect { get; set; }
        [Range(0, int.MaxValue)]
        public int AttentionTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int ImitationCorrect { get; set; }
        [Range(0, int.MaxValue)]
        public int ImitationTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int ContactCorrect { get; set; }
        [Range(0, int.MaxValue)]
        public int ContactTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int DeskActivitiesCorrect { get; set; }
        [Range(0, int.MaxValue)]
        public int DeskActivitiesTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int IndependenceCorrect { get; set; }
        [Range(0, int.MaxValue)]
        public int IndependenceTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int TimeRegistered { get; set; }
        [Range(0, int.MaxValue)]
        public int TimeTotal { get; set; }

        [StringLength(500)]
        public string? ProtocolNotes { get; set; }
    }
}
