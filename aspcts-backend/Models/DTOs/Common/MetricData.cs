using System;

namespace aspcts_backend.Models.DTOs.Common
{
    public class MetricData
    {
        public int Correct { get; set; }
        public int Total { get; set; }
        public decimal Percentage => Total > 0 ? Math.Round((decimal)Correct / Total * 100, 1) : 0;
    }
}