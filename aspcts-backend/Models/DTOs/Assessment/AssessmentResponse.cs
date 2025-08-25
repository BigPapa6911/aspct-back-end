using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Assessment
{
    public class AssessmentResponse
    {
        public Guid AssessmentId { get; set; }
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public Guid PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
        public DateTime AssessmentDate { get; set; }
        public int? OverallScore { get; set; }
        public string AssessmentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}