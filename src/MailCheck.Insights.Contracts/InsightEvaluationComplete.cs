using System;
using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.Insights.Contracts
{
    public class InsightEvaluationComplete : Message
    {
        public InsightEvaluationComplete(string id, DateTime calculatedAt, List<Insight> insights, DateTime startDate, DateTime endDate) : base(id)
        {
            CalculatedAt = calculatedAt;
            Insights = insights;
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime CalculatedAt { get; set; }

        public List<Insight> Insights { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
