using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class Child
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChildID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(255)]
        public string Diagnosis { get; set; }

        public DateTime OnboardingDate { get; set; }

        [ForeignKey("AssignedPsychologist")]
        public int AssignedPsychologistID { get; set; }
        public Psychologist AssignedPsychologist { get; set; }

        [ForeignKey("Parent")]
        public int ParentID { get; set; }
        public Parent Parent { get; set; }

        // Relacionamentos
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Assessment> Assessments { get; set; }
        public ICollection<InterventionPlan> InterventionPlans { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<CommunicationMessage> SentMessages { get; set; }
        public ICollection<CommunicationMessage> ReceivedMessages { get; set; }
        public ICollection<ChildSupportingSkillProgress> ChildSupportingSkillProgress { get; set; }
        public ICollection<ChildResource> ChildResources { get; set; }
    }
}