using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Assessment
{
    public class AssessmentRequest
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public DateTime AssessmentDate { get; set; }

        [Required]
        public string AssessmentType { get; set; } = string.Empty; // "Milestones", "Barriers", "Transition"
    }
}