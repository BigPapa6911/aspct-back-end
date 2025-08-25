using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class InterventionPlan
    {
        [Key]
        public Guid PlanId { get; set; } = Guid.NewGuid();

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }
        public Child Child { get; set; } = null!;

        [ForeignKey("Psychologist")]
        public Guid PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Goals { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // "Active", "Archived", "Completed"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<InterventionGoal> InterventionGoals { get; set; } = new List<InterventionGoal>();
    }
}