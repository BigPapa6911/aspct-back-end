using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class Resource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; }

        // Relacionamento muitos-para-muitos
        public ICollection<ChildResource> ChildResources { get; set; }
    }
}