using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public abstract class Assessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssessmentID { get; set; }

        [ForeignKey("Child")]
        public int ChildID { get; set; }
        public Child Child { get; set; }

        [ForeignKey("Psychologist")]
        public int PsychologistID { get; set; }
        public Psychologist Psychologist { get; set; }

        [Required]
        public DateTime AssessmentDate { get; set; }

        public decimal OverallScore { get; set; }
    }
}