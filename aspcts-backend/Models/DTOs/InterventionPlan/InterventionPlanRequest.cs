using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.InterventionPlan
{
    public class InterventionPlanRequest
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Goals { get; set; }

        public List<InterventionGoalRequest> InterventionGoals { get; set; } = new();
    }
}