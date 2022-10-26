using System;
using System.Threading.Tasks;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators
{
    public interface IInsightGenerator
    {
        Task<Insight> GenerateInsights(string id, DateTime startDate, DateTime endDate);
    }
}
