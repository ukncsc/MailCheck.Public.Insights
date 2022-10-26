using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Contracts
{
    public class Insight
    {
        public InsightType Type { get; set; }
        public List<NamedAdvisory> Advisories { get; set; }

        public Insight(InsightType type, List<NamedAdvisory> advisories)
        {
            Type = type;
            Advisories = advisories;
        }
    }
}