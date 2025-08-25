using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionUpdateRequest
    {
        public DateTime? SessionDate { get; set; }

        [Range(1, 480)]
        public int? Duration { get; set; }

        [StringLength(100)]
        public string? SessionType { get; set; }

        public string? NotesWhatWasDone { get; set; }

        public string? NotesWhatWasDiagnosed { get; set; }

        public string? NotesWhatWillBeDone { get; set; }

        public bool? IsSharedWithParent { get; set; }
    }
}