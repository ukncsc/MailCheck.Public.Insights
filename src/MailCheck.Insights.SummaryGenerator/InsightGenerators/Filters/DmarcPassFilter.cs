using System;
using System.Collections.Generic;
using System.Linq;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public class DmarcPassFilter : IFilter
    {
        public FilterType FilterType => FilterType.Mandatory;

        public bool Filter(IFilterable source)
        {
            return source.Dkim == "pass" || source.Spf == "pass";
        }
    }
}