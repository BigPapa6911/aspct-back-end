using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class TransitionAssessment : Assessment
    {
        // JSON column para armazenar pontuações das 18 áreas de transição
        [Column(TypeName = "nvarchar(max)")]
        public string TransitionScores { get; set; } = "{}"; // JSON: {"area1": 2, "area2": 3, ...}

        [Column(TypeName = "nvarchar(max)")]
        public string? ReadinessNotes { get; set; }

        public TransitionAssessment()
        {
            AssessmentType = "Transition";
        }
    }
}