using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models
{
    public class Psychologist : User
    {
        public int PsychologistID { get; set; }

        [MaxLength(50)]
        public string LicenseNumber { get; set; }

        [MaxLength(100)]
        public string Specialization { get; set; }

        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [MaxLength(100)]
        public string ClinicName { get; set; }

        // Relacionamentos
        public ICollection<Child> Children { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Assessment> Assessments { get; set; }
        public ICollection<InterventionPlan> InterventionPlans { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<CommunicationMessage> SentMessages { get; set; }
        public ICollection<CommunicationMessage> ReceivedMessages { get; set; }
    }
}