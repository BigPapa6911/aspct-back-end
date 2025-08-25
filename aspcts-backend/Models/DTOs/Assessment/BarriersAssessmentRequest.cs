using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Assessment
{
    public class BarriersAssessmentRequest : AssessmentRequest
    {
        [Required]
        public Dictionary<string, int> BarrierScores { get; set; } = new();

        public string? QualitativeNotes { get; set; }

        public BarriersAssessmentRequest()
        {
            AssessmentType = "Barriers";
        }
    }
}