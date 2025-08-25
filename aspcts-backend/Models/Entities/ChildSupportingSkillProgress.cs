using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class ChildSupportingSkillProgress
    {
        [Key]
        public Guid ProgressId { get; set; } = Guid.NewGuid();

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }
        public Child Child { get; set; } = null!;

        [ForeignKey("SupportingSkill")]
        public Guid SkillId { get; set; }
        public SupportingSkill SupportingSkill { get; set; } = null!;

        [StringLength(50)]
        public string Status { get; set; } = "Not Started"; // "Not Started", "In Progress", "Mastered"

        public DateTime? StartDate { get; set; }
        public DateTime? MasteryDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}