using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models
{
    public class Parent : User
    {
        public int ParentID { get; set; }

        [MaxLength(50)]
        public string ChildRelationship { get; set; }

        [MaxLength(20)]
        public string ContactNumber { get; set; }

        // Relacionamentos
        public ICollection<Child> Children { get; set; }
        public ICollection<CommunicationMessage> SentMessages { get; set; }
        public ICollection<CommunicationMessage> ReceivedMessages { get; set; }
    }
}