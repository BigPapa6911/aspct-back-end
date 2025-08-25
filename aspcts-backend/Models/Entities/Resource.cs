using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.Entities
{
    public class Resource
    {
        [Key]
        public Guid ResourceId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // "PDF", "Video", "Link", "Document"

        [StringLength(500)]
        public string? FilePath { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Relacionamento Many-to-Many com Child seria implementado via tabela de junção
        // Para simplicidade, pode ser implementado posteriormente
    }
}