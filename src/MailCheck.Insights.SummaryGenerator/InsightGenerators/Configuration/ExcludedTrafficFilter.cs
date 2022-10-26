using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.Insights.Contracts.Raw;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public interface IExcludedTrafficFilter
    {
        List<NormalisedRecord> Filter(List<NormalisedRecord> source);
    }

    public class ExcludedTrafficFilter : IExcludedTrafficFilter
    {
        private readonly List<string> _excludedProviders = new List<string> { "lsoft.se", "groups.io" };
        private readonly List<string> _excludedHostNames = new List<string> { "Mismatch", "Unknown" };

        public List<NormalisedRecord> Filter(List<NormalisedRecord> source)
        {
            List<NormalisedRecord> filteredRecords = source.Where(record =>
                !_excludedProviders.Contains(record.HostProvider, StringComparer.InvariantCultureIgnoreCase) &&
                !_excludedHostNames.Contains(record.HostName, StringComparer.InvariantCultureIgnoreCase) &&
                record.Arc == 0 &&
                record.ProxyBlockListCount + record.SuspiciousNetworkBlockListCount +
                record.HijackedNetworkBlockListCount + record.EndUserNetworkBlockListCount +
                record.SpamSourceBlockListCount + record.MalwareBlockListCount + record.EndUserBlockListCount +
                record.BounceReflectorBlockListCount == 0).ToList();

            return filteredRecords;
        }
    }
}
