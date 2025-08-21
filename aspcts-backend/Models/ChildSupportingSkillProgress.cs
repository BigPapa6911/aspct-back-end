using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class ChildSupportingSkillProgress
    {
        [Key]
        public int ChildID { get; set; }

        [Key]
        public int SkillID { get; set; }

        [ForeignKey("Child")]
        public Child Child { get; set; }

        [ForeignKey("SupportingSkill")]
        public SupportingSkill SupportingSkill { get; set; }

        public string Status { get; set; }
    }
}