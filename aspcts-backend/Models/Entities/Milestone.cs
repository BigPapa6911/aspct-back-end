using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.Entities
{
    public class Milestone
    {
        [Key]
        public Guid MilestoneId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 3)]
        public int DevelopmentalLevel { get; set; } // 1, 2, ou 3

        [StringLength(100)]
        public string Domain { get; set; } = string.Empty; // "Mand", "Tact", "Listener", etc.

        public int Order { get; set; } // ordem dentro do nível/domínio

        // Navigation properties
        public ICollection<TaskAnalysisStep> TaskAnalysisSteps { get; set; } = new List<TaskAnalysisStep>();
    }
}