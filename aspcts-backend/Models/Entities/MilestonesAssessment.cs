using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace aspcts_backend.Models.Entities
{
    public class MilestonesAssessment : Assessment
    {
    public int? Level1Score { get; set; }
    public int? Level2Score { get; set; }
    public int? Level3Score { get; set; }
    
    // JSON column para armazenar pontuações individuais dos marcos
    [Column(TypeName = "nvarchar(max)")]
    public string MilestoneScores { get; set; } = "{}"; // JSON: {"milestone1": 2, "milestone2": 1, ...}
    
    public MilestonesAssessment()
    {
        AssessmentType = "Milestones";
    }
    }
}