using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Session
    {
        [Key]
        public Guid SessionId { get; set; } = Guid.NewGuid();

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }
        public Child Child { get; set; } = null!;

        [ForeignKey("Psychologist")]
        public Guid PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;

        public DateTime SessionDate { get; set; }

        public int Duration { get; set; } // em minutos

        [StringLength(100)]
        public string SessionType { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? NotesWhatWasDone { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? NotesWhatWasDiagnosed { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? NotesWhatWillBeDone { get; set; }

        public bool IsSharedWithParent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}