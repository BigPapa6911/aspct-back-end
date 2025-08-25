using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionResponse
    {
        public Guid SessionId { get; set; }
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public Guid PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public int Duration { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public string? NotesWhatWasDone { get; set; }
        public string? NotesWhatWasDiagnosed { get; set; }
        public string? NotesWhatWillBeDone { get; set; }
        public bool IsSharedWithParent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Parent-friendly summary
        public string? ParentSummary => IsSharedWithParent ? GetParentFriendlySummary() : null;

        private string GetParentFriendlySummary()
        {
            var summary = new List<string>();

            if (!string.IsNullOrEmpty(NotesWhatWasDone))
                summary.Add($"Atividades realizadas: {NotesWhatWasDone}");

            if (!string.IsNullOrEmpty(NotesWhatWasDiagnosed))
                summary.Add($"Observações: {NotesWhatWasDiagnosed}");

            if (!string.IsNullOrEmpty(NotesWhatWillBeDone))
                summary.Add($"Próximos passos: {NotesWhatWillBeDone}");

            return string.Join("\n\n", summary);
        }
    }
}