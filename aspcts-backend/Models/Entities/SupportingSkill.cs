using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.Entities
{
    public class SupportingSkill
    {
        [Key]
        public Guid SkillId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string Domain { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<ChildSupportingSkillProgress> ChildProgresses { get; set; } = new List<ChildSupportingSkillProgress>();
    }
}