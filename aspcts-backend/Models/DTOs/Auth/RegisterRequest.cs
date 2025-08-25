using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // "Psychologist" or "Parent"

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        // Campos específicos para Psicólogo
        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(200)]
        public string? ClinicName { get; set; }

        // Campos específicos para Responsável
        [StringLength(50)]
        public string? ChildRelationship { get; set; }
    }
}