using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models
{
    public class ChildResource
    {
        [Key]
        public int ChildID { get; set; }

        [Key]
        public int ResourceID { get; set; }

        [ForeignKey("Child")]
        public Child Child { get; set; }

        [ForeignKey("Resource")]
        public Resource Resource { get; set; }
    }
}