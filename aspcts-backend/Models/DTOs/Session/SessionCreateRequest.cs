using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionCreateRequest
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Range(1, 480)] // 1 a 480 minutos (8 horas)
        public int Duration { get; set; } = 60;

        [Required]
        [StringLength(100)]
        public string SessionType { get; set; } = string.Empty;

        public string? NotesWhatWasDone { get; set; }

        public string? NotesWhatWasDiagnosed { get; set; }

        public string? NotesWhatWillBeDone { get; set; }

        public bool IsSharedWithParent { get; set; } = false;
    }
}