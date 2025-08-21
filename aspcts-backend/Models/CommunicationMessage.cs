using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class CommunicationMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }

        [ForeignKey("Sender")]
        public int SenderID { get; set; }
        public User Sender { get; set; }

        [ForeignKey("Receiver")]
        public int ReceiverID { get; set; }
        public User Receiver { get; set; }

        [ForeignKey("Child")]
        public int ChildID { get; set; }
        public Child Child { set; get; }

        public DateTime Timestamp { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }
    }
}