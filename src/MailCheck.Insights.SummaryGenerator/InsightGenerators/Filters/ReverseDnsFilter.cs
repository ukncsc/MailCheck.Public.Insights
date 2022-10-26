using System;
using System.Collections.Generic;
using System.Linq;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public class ReverseDnsFilter : IFilter
    {
        private readonly List<string> _reverseDnsCheckFailedHosts = new List<string> { "Mismatch", "Unknown" };
        private readonly List<string> _reverseDnsCheckExemptProviders = new List<string> { "NHS N3 network" };

        public FilterType FilterType => FilterType.NonMandatory;

        public bool Filter(IFilterable source)
        {
            bool passedReverseDns = _reverseDnsCheckExemptProviders.Contains(source.HostProvider, StringComparer.InvariantCultureIgnoreCase) ||
                                    !_reverseDnsCheckFailedHosts.Contains(source.HostName, StringComparer.InvariantCultureIgnoreCase);

            return passedReverseDns;
        }
    }
}