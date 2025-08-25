using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Assessment
{
    public class MilestonesAssessmentRequest : AssessmentRequest
    {
        public int? Level1Score { get; set; }
        public int? Level2Score { get; set; }
        public int? Level3Score { get; set; }

        [Required]
        public Dictionary<string, int> MilestoneScores { get; set; } = new();

        public MilestonesAssessmentRequest()
        {
            AssessmentType = "Milestones";
        }
    }
}