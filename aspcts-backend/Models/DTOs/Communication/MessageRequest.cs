using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Communication
{
    public class MessageRequest
    {
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Subject { get; set; }

        public string MessageType { get; set; } = "General";
    }
}