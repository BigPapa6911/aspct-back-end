using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class SupportingSkill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SkillID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string Domain { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        // Relacionamento muitos-para-muitos
        public ICollection<ChildSupportingSkillProgress> ChildSupportingSkillProgress { get; set; }
    }
}