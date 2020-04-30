using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.HealthReport
{
    public class HealthReport
    {
        public string CreationDate { get; set; } = string.Empty;
        public string CreationTime { get; set; } = string.Empty;
        public List<string> DiagnosticMessages { get; set; } = new List<string>();
        public bool HasValidData { get; set; }
        public string ReportId { get; set; } = string.Empty;
        public bool IsOlderSixMonths { get; set; }
        public string MileageValue { get; set; } = string.Empty;
        public long Timestamp { get; set; }
        public HeaderData HeaderData { get; set; } = new HeaderData();
        public string JobStatus { get; set; } = string.Empty;
        public bool FetchFailed { get; set; }
    }

    public class HeaderData
    {
        public string RangeMileage { get; set; } = string.Empty;
        public List<string> LastRefreshTime { get; set; } = new List<string>();
        public bool ServiceOverdue { get; set; }
        public bool OilOverdue { get; set; }
        public string Mileage { get; set; } = string.Empty;
        public bool ShowOil { get; set; }
        public string VhrNoDataText { get; set; } = string.Empty;
        public bool ShowService { get; set; }
    }
}