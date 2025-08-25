using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Assessment
{
    public class TransitionAssessmentRequest : AssessmentRequest
    {
        [Required]
        public Dictionary<string, int> TransitionScores { get; set; } = new();

        public string? ReadinessNotes { get; set; }

        public TransitionAssessmentRequest()
        {
            AssessmentType = "Transition";
        }
    }
}