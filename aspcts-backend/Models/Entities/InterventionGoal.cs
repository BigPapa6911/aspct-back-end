using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class InterventionGoal
    {
        [Key]
        public Guid GoalId { get; set; } = Guid.NewGuid();

        [ForeignKey("InterventionPlan")]
        public Guid PlanId { get; set; }
        public InterventionPlan InterventionPlan { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(300)]
        public string? TargetBehavior { get; set; }

        [StringLength(300)]
        public string? MeasurementCriteria { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ProgressNotes { get; set; }

        public DateTime? TargetDate { get; set; }
        public DateTime? AchievedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // "Active", "Achieved", "Modified", "Discontinued"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}