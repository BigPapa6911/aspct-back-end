using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.InterventionPlan
{
    public class InterventionGoalResponse
    {
        public Guid GoalId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? TargetBehavior { get; set; }
        public string? MeasurementCriteria { get; set; }
        public string? ProgressNotes { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? AchievedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}