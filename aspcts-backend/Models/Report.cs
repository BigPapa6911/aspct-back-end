using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportID { get; set; }

        [ForeignKey("Child")]
        public int ChildID { get; set; }
        public Child Child { get; set; }

        [ForeignKey("Psychologist")]
        public int PsychologistID { get; set; }
        public Psychologist Psychologist { get; set; }

        public DateTime ReportDate { get; set; }

        [MaxLength(50)]
        public string ReportType { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; }

        public string SummaryForParent { get; set; }
    }
}