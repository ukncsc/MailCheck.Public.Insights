using System;
using System.Collections.Generic;

namespace MailCheck.Insights.Contracts
{
    public class InsightEntityState
    {
        public string Id { get; set; }

        public DateTime? CalculatedAt { get; set; }

        public List<Insight> Insights { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Version { get; set; }
    }
}