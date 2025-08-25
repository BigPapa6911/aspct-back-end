using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Communication
{
    public class MessageResponse
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverRole { get; set; } = string.Empty;
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? Subject { get; set; }
        public string MessageType { get; set; } = string.Empty;
    }
}