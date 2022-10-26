using System.Collections.Generic;
using MailCheck.Insights.Contracts.Subdomains;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains
{
    public class SubdomainsEvaluationObject
    {
        public SubdomainsEvaluationObject(string domain, List<SubdomainsData> data = null)
        {
            Domain = domain;
            Data = data ?? new List<SubdomainsData>();
        }
        public string Domain { get; }
        public List<SubdomainsData> Data { get; }
    }
}
