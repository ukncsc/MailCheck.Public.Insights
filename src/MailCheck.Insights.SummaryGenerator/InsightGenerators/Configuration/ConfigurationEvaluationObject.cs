using System;
using System.Collections.Generic;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public class ConfigurationEvaluationObject
    {
        public string Domain { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Url { get; set; }
        public int TotalTraffic { get; set; }
        public decimal PercentNone { get; set; }
        public decimal PercentQuarantineOrReject { get; set; }
        public decimal PercentFailingDmarc { get; set; }
        public List<ProviderCalculation> ProviderCalculations { get; set; }
        public string LatestPolicy { get; set; }
        public decimal TotalMixedQuarantineOrReject { get; set; }
    }

    public class ProviderCalculation
    {
        public string Name { get; set; }
        public int TotalMail { get; set; }
        public decimal PercentNone { get; set; }
        public decimal PercentQuarantineOrReject { get; set; }
        public decimal PercentDmarcFail { get; set; }
        public decimal PercentSpfAuthFail { get; set; }
        public decimal PercentSpfMisaligned { get; set; }
        public decimal PercentDkimAuthFail { get; set; }
        public decimal PercentDkimMisaligned { get; set; }

    }
}
