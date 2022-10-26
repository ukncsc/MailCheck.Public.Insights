using System;

namespace MailCheck.Insights.Contracts.Abuse
{
    public class AbuseData
    {
        public string Domain { get; set; }
        public string ReverseDomain { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int Count { get; set; }
        public int Flagged { get; set; }
        public string P { get; set; }
        public int? Pct { get; set; }
        public string RecordId { get; set; }
    }
}