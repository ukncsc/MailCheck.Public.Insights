namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public class BlocklistFilter : IFilter
    {
        public FilterType FilterType => FilterType.NonMandatory;

        public bool Filter(IFilterable source)
        {
            bool passedBlocklistCheck =
                source.BounceReflectorBlocklistCount == 0 &&
                source.EndUserBlocklistCount == 0 &&
                source.EndUserNetworkBlocklistCount == 0 &&
                source.HijackedNetworkBlocklistCount == 0 &&
                source.MalwareBlocklistCount == 0 &&
                source.ProxyBlocklistCount == 0 &&
                source.SpamSourceBlocklistCount == 0 &&
                source.SuspiciousNetworkBlocklistCount == 0;

            return passedBlocklistCheck;
        }
    }
}