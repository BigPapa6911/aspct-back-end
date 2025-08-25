using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.InterventionPlan
{
    public class InterventionPlanResponse
    {
        public Guid PlanId { get; set; }
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public Guid PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Goals { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<InterventionGoalResponse> InterventionGoals { get; set; } = new();
    }
}