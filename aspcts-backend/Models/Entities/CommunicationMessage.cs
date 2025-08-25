using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class CommunicationMessage
    {
        [Key]
    public Guid MessageId { get; set; } = Guid.NewGuid();
    
    [ForeignKey("Sender")]
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;
    
    [ForeignKey("Receiver")]
    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
    
    [ForeignKey("Child")]
    public Guid ChildId { get; set; }
    public Child Child { get; set; } = null!;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    [StringLength(100)]
    public string? Subject { get; set; }
    
    [StringLength(50)]
    public string MessageType { get; set; } = "General"; // "General", "SessionUpdate", "Report", etc.
    }
}