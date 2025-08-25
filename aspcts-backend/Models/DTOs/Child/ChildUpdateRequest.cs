using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Child
{
    public class ChildUpdateRequest
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Diagnosis { get; set; }

        public Guid? PrimaryParentId { get; set; }

        public Guid? SecondaryParentId { get; set; }

        public string? MedicalHistory { get; set; }

        public bool? IsActive { get; set; }
    }
}