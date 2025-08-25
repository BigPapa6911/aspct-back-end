using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.InterventionPlan
{
    public class InterventionGoalRequest
    {
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(300)]
        public string? TargetBehavior { get; set; }

        [StringLength(300)]
        public string? MeasurementCriteria { get; set; }

        public string? ProgressNotes { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Status { get; set; } = "Active";
    }
}