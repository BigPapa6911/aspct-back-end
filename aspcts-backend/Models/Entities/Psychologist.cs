using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Psychologist
    {
        [Key]
    public Guid PsychologistId { get; set; } = Guid.NewGuid();
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [StringLength(50)]
    public string? LicenseNumber { get; set; }
    
    [StringLength(100)]
    public string? Specialization { get; set; }
    
    [StringLength(200)]
    public string? ClinicName { get; set; }
    
    // Navigation properties
    public ICollection<Child> Children { get; set; } = new List<Child>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    public ICollection<InterventionPlan> InterventionPlans { get; set; } = new List<InterventionPlan>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}