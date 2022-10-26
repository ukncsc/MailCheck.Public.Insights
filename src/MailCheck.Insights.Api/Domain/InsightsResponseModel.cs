using System;
using System.Collections.Generic;

namespace MailCheck.Insights.Api.Domain
{
    public class InsightsResponseModel
    {
        public bool Ignored { get; set; }
        public DateTime? CalculatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
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
            public string MarkDown { get; set; }
        }
    }
}