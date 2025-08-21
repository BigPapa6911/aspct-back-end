using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models
{
    public class TransitionAssessment : Assessment
    {
        public string TransitionScoresJson { get; set; } // Armazenar como JSON
    }
}