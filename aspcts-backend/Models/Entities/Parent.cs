using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspcts_backend.Models.Entities
{
    public class Parent
    {
        [Key]
    public Guid ParentId { get; set; } = Guid.NewGuid();
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [StringLength(50)]
    public string ChildRelationship { get; set; } = string.Empty; // "Father", "Mother", "Guardian", etc.
    
    // Navigation properties
    public ICollection<Child> PrimaryChildren { get; set; } = new List<Child>();
    public ICollection<Child> SecondaryChildren { get; set; } = new List<Child>();
    }
}