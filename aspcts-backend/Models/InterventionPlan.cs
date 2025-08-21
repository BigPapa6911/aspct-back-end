using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class InterventionPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlanID { get; set; }

        [ForeignKey("Child")]
        public int ChildID { get; set; }
        public Child Child { get; set; }

        [ForeignKey("Psychologist")]
        public int PsychologistID { get; set; }
        public Psychologist Psychologist { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Goals { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        // Relacionamento
        public ICollection<InterventionGoal> InterventionGoals { get; set; }
    }
}