using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Child
    {
        [Key]
        public Guid ChildId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Diagnosis { get; set; }

        public DateTime OnboardingDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [ForeignKey("AssignedPsychologist")]
        public Guid AssignedPsychologistId { get; set; }
        public Psychologist AssignedPsychologist { get; set; } = null!;

        [ForeignKey("PrimaryParent")]
        public Guid PrimaryParentId { get; set; }
        public Parent PrimaryParent { get; set; } = null!;

        [ForeignKey("SecondaryParent")]
        public Guid? SecondaryParentId { get; set; }
        public Parent? SecondaryParent { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? MedicalHistory { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
        public ICollection<InterventionPlan> InterventionPlans { get; set; } = new List<InterventionPlan>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<CommunicationMessage> CommunicationMessages { get; set; } = new List<CommunicationMessage>();
        public ICollection<ChildSupportingSkillProgress> SupportingSkillProgresses { get; set; } = new List<ChildSupportingSkillProgress>();
    }
}