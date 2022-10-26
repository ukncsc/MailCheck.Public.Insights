using System.Collections.Generic;
using System.Linq;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Api.Service
{
    public interface IInsightsSummaryResponseModelFactory
    {
        InsightsSummaryResponseModel Create(InsightEntityState state, bool hasReportingData);
    }

    public class InsightsSummaryResponseModelFactory : IInsightsSummaryResponseModelFactory
    {
        public InsightsSummaryResponseModel Create(InsightEntityState state, bool hasReportingData)
        {
            if (hasReportingData == false && state == null)
            {
                return null;
            }

            if (hasReportingData == false)
            {
                return new InsightsSummaryResponseModel { HasReportingData = false };
            }

            if (state == null)
            {
                return new InsightsSummaryResponseModel { HasReportingData = true };
            }

            List<InsightsSummaryResponseModel.InsightGroup> insightGroups = state.Insights
                .GroupBy(x => x.Type)
                .Select(x => new InsightsSummaryResponseModel.InsightGroup
                {
                    InsightType = x.Key.ToString(),
                    InsightAdvisories = x.SelectMany(y => y.Advisories).Select(z =>
                        new InsightsSummaryResponseModel.InsightAdvisory
                        {
                            AdvisoryType = z.MessageType.ToString().ToLower(),
                            Text = z.Text
                        }).ToList()
                })
                .ToList();

            InsightsSummaryResponseModel insightsResponseModel = new InsightsSummaryResponseModel
            {
                InsightGroups = insightGroups,
                HasReportingData = true
            };

            return insightsResponseModel;
        }
    }
}