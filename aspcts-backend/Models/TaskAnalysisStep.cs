using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class TaskAnalysisStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StepID { get; set; }

        [ForeignKey("Milestone")]
        public int MilestoneID { get; set; }
        public Milestone Milestone { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public DateTime? MasteryDate { get; set; }
    }
}