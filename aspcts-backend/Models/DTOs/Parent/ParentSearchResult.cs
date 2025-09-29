using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Parent
{
    public class ParentSearchResult
    {
        public Guid ParentId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string ChildRelationship { get; set; } = string.Empty;
    }
    public class ParentDetailInfo : ParentSearchResult
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public int ChildrenCount { get; set; }
    }
}