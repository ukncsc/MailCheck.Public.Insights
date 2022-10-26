using System;
using System.Collections.Generic;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse
{
    public class AbuseEvaluationObject
    {
        public string Domain { get; set; }
        public List<FlaggedTrafficData> FlaggedTrafficData { get; set; }
        public List<FlaggedTrafficData> FlaggedSubdomainTrafficData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Url { get; set; }
    }
}