using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionID { get; set; }

        [ForeignKey("Child")]
        public int ChildID { get; set; }
        public Child Child { get; set; }

        [ForeignKey("Psychologist")]
        public int PsychologistID { get; set; }
        public Psychologist Psychologist { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        public int Duration { get; set; } // Duração em minutos

        [MaxLength(50)]
        public string SessionType { get; set; }

        public string NotesWhatWasDone { get; set; }

        public string NotesWhatWasDiagnosed { get; set; }

        public string NotesWhatWillBeDone { get; set; }

        public bool IsSharedWithParent { get; set; }
    }
}