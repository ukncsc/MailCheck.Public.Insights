namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public interface IFilterable
    {
        int Count { get; set; }
        string HostName { get; set; }
        string HostProvider { get; set; }
        int BounceReflectorBlocklistCount { get; set; }
        int EndUserBlocklistCount { get; set; }
        int EndUserNetworkBlocklistCount { get; set; }
        int HijackedNetworkBlocklistCount { get; set; }
        int MalwareBlocklistCount { get; set; }
        int ProxyBlocklistCount { get; set; }
        int SpamSourceBlocklistCount { get; set; }
        int SuspiciousNetworkBlocklistCount { get; set; }
        string P { get; set; }
        int Pct { get; set; }
        string Disposition { get; set; }
        string Dkim { get; set; }
        string Spf { get; set; }
        int Arc { get; set; }
        int Forwarded { get; set; }
        int LocalPolicy { get; set; }
        int MailingList { get; set; }
        int OtherOverrideReason { get; set; }
        int TrustedForwarder { get; set; }
    }
}