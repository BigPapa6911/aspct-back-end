using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class TaskAnalysisStep
    {
        [Key]
        public Guid StepId { get; set; } = Guid.NewGuid();

        [ForeignKey("Milestone")]
        public Guid MilestoneId { get; set; }
        public Milestone Milestone { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Order { get; set; }

        public DateTime? MasteryDate { get; set; }

        public bool IsMastered { get; set; } = false;
    }
}