using System.Collections.Generic;
using System.Linq;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Api.Service
{
    public interface IInsightsResponseModelFactory
    {
        InsightsResponseModel Create(InsightEntityState state);
    }

    public class InsightsResponseModelFactory : IInsightsResponseModelFactory
    {
        public InsightsResponseModel Create(InsightEntityState state)
        {
            List<InsightsResponseModel.InsightGroup> insightGroups = state.Insights
                .GroupBy(x => x.Type)
                .Select(x => new InsightsResponseModel.InsightGroup
                {
                    InsightType = x.Key.ToString(),
                    InsightAdvisories = x.SelectMany(y => y.Advisories).Select(z =>
                        new InsightsResponseModel.InsightAdvisory
                        {
                            AdvisoryType = z.MessageType.ToString().ToLower(),
                            MarkDown = z.MarkDown,
                            Text = z.Text
                        }).ToList()
                })
                .ToList();

            InsightsResponseModel insightsResponseModel = new InsightsResponseModel
            {
                CalculatedAt = state.CalculatedAt,
                StartDate = state.StartDate,
                EndDate = state.EndDate,
                InsightGroups = insightGroups
            };

            return insightsResponseModel;
        }
    }
}