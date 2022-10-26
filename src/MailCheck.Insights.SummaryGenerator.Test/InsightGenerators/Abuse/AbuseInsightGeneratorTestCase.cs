using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Abuse
{
    public class AbuseInsightGeneratorTestCase
    {
        public string Description { get; set; }
        public List<FlaggedTrafficData> Data { get; set; }
        public List<FlaggedTrafficData> SubdomainData { get; set; }
        public List<AdvisoryMessage> ExpectedAdvisories { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}