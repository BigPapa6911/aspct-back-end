using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models
{
    public class MilestonesAssessment : Assessment
    {
        public int Level1Score { get; set; }
        public int Level2Score { get; set; }
        public int Level3Score { get; set; }

        public string MilestoneScoresJson { get; set; } // Armazenar como JSON
    }
}