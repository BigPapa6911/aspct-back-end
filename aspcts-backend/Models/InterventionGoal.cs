using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using aspcts_backend.Models;

namespace aspcts_backend.Models
{
    public class InterventionGoal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoalID { get; set; }

        [ForeignKey("InterventionPlan")]
        public int PlanID { get; set; }
        public InterventionPlan InterventionPlan { get; set; }

        public string Description { get; set; }

        public string TargetBehavior { get; set; }

        public string MeasurementCriteria { get; set; }

        public string ProgressNotes { get; set; }
    }
}