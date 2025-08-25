using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Child
{
    public class ChildResponse
    {
        public Guid ChildId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public DateTime DateOfBirth { get; set; }
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        public string Gender { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public DateTime OnboardingDate { get; set; }
        public bool IsActive { get; set; }

        // Psychologist info
        public Guid AssignedPsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;

        // Parents info
        public ParentInfo PrimaryParent { get; set; } = new();
        public ParentInfo? SecondaryParent { get; set; }

        public string? MedicalHistory { get; set; }

        // Statistics
        public int TotalSessions { get; set; }
        public int TotalAssessments { get; set; }
        public DateTime? LastSessionDate { get; set; }
        public DateTime? LastAssessmentDate { get; set; }
    }

    public class ParentInfo
    {
        public Guid ParentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string ChildRelationship { get; set; } = string.Empty;
    }
}