using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse
{
    public class RawAbuseData : IFilterable
    {
        public int Count { get; set; }
        public string HostName { get; set; }
        public string HostProvider { get; set; }
        public int BounceReflectorBlocklistCount { get; set; }
        public int EndUserBlocklistCount { get; set; }
        public int EndUserNetworkBlocklistCount { get; set; }
        public int HijackedNetworkBlocklistCount { get; set; }
        public int MalwareBlocklistCount { get; set; }
        public int ProxyBlocklistCount { get; set; }
        public int SpamSourceBlocklistCount { get; set; }
        public int SuspiciousNetworkBlocklistCount { get; set; }
        public string P { get; set; }
        public int Pct { get; set; }
        public string Disposition { get; set; }
        public string Dkim { get; set; }
        public string Spf { get; set; }
        public int Arc { get; set; }
        public int Forwarded { get; set; }
        public int LocalPolicy { get; set; }
        public int MailingList { get; set; }
        public int OtherOverrideReason { get; set; }
        public int TrustedForwarder { get; set; }
    }
}