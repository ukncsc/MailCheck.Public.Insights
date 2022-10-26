using System.Collections.Generic;

namespace MailCheck.Insights.Api.Domain
{
    public class InsightsSummaryResponseModel
    {
        public bool HasReportingData { get; set; }

        public List<InsightGroup> InsightGroups { get; set; }

        public class InsightGroup
        {
            public string InsightType { get; set; }
            public List<InsightAdvisory> InsightAdvisories { get; set; }
        }

        public class InsightAdvisory
        {
            public string AdvisoryType { get; set; }
            public string Text { get; set; }
        }
    }
}