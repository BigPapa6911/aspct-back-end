using System;
using System.Collections.Generic;
using System.Linq;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionProtocolDataResponse
    {
        public Guid ProtocolDataId { get; set; }
        public Guid SessionId { get; set; }
        public int TotalDuration { get; set; }
        public List<ProtocolRecordResponse> Records { get; set; } = new();
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProtocolRecordResponse
    {
        public Guid RecordId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<TimeIntervalResponse> Intervals { get; set; } = new();
        public int TotalCorrect => Intervals.Sum(i => i.Correct);
        public int TotalIncorrect => Intervals.Sum(i => i.Incorrect);
        public decimal SuccessRate 
        { 
            get
            {
                var total = TotalCorrect + TotalIncorrect;
                return total > 0 ? Math.Round((decimal)TotalCorrect / total * 100, 1) : 0;
            }
        }
    }

    public class TimeIntervalResponse
    {
        public int Minutes { get; set; }
        public int Correct { get; set; }
        public int Incorrect { get; set; }
    }
}
