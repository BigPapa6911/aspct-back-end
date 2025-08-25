using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Assessment
    {
        [Key]
        public Guid AssessmentId { get; set; } = Guid.NewGuid();

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }
        public Child Child { get; set; } = null!;

        [ForeignKey("Psychologist")]
        public Guid PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;

        public DateTime AssessmentDate { get; set; }

        public int? OverallScore { get; set; }

        [StringLength(50)]
        public string AssessmentType { get; set; } = string.Empty; // "Milestones", "Barriers", "Transition"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}