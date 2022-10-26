using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains;

namespace MailCheck.Insights.SummaryGenerator.Test.Configuration
{
    public class SubdomainsInsightGeneratorTestCase
    {
        public string Description { get; set; }
        public SubdomainsRawData Data { get; set; }
        public List<AdvisoryMessage> ExpectedAdvisories { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}