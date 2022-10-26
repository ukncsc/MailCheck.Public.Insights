namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public class DmarcOverrideFilter : IFilter
    {
        public FilterType FilterType => FilterType.Override;

        public bool Filter(IFilterable source)
        {
            bool overrideExists =
                source.Arc +
                source.Forwarded +
                source.LocalPolicy +
                source.MailingList +
                source.OtherOverrideReason +
                source.TrustedForwarder > 0;

            return overrideExists;
        }
    }
}
