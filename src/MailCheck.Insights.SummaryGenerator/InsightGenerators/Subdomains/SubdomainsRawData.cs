using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.Contracts.Subdomains;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains
{
    public class SubdomainsRawData
    {
        public SubdomainsRawData(string domain, string p, string sp, List<NormalisedRecord> data)
        {
            Domain = domain;
            ParentPolicy = p;
            ParentSubPolicy = sp;
            Subdomains = data ?? new List<NormalisedRecord>();
        }

        public string Domain { get; }
        public string ParentPolicy { get; }
        public string ParentSubPolicy { get; }
        public List<NormalisedRecord> Subdomains { get; }
    }
}
