using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models
{
    public class BarrierAssessment : Assessment
    {
        public string BarrierScoresJson { get; set; } // Armazenar como JSON
        
    }
}