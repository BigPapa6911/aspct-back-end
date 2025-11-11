using System;
using aspcts_backend.Models.DTOs.Common;

namespace aspcts_backend.Models.DTOs.Session
{
    public class SessionProtocolDataResponse
    {
        public Guid ProtocolDataId { get; set; }
        public int TotalTrials { get; set; }
        
        public MetricData Attention { get; set; } = new();
        public MetricData Imitation { get; set; } = new();
        public MetricData Contact { get; set; } = new();
        public MetricData DeskActivities { get; set; } = new();
        public MetricData Independence { get; set; } = new();
        public MetricData Time { get; set; } = new();
        
        public string? ProtocolNotes { get; set; }
    }

}