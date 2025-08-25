using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class BarriersAssessment : Assessment
    {
        // JSON column para armazenar pontuações individuais das barreiras
        [Column(TypeName = "nvarchar(max)")]
        public string BarrierScores { get; set; } = "{}"; // JSON: {"barrier1": 3, "barrier2": 2, ...}

        [Column(TypeName = "nvarchar(max)")]
        public string? QualitativeNotes { get; set; }

        public BarriersAssessment()
        {
            AssessmentType = "Barriers";
        }
    }
}