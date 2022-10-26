using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse.EvaluationRules
{
    public class AbuseRateWithAllPolicies : IRule<AbuseEvaluationObject>
    {
        public Guid AbuseGuid = new Guid("917c7b26-ec74-4474-bd31-241f6b3b752f");

        public int SequenceNo => 10;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(AbuseEvaluationObject abuseEvaluationObject)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();
            List<FlaggedTrafficData> flaggedTrafficData = abuseEvaluationObject.FlaggedTrafficData ?? new List<FlaggedTrafficData>();
            List<FlaggedTrafficData> flaggedSubdomainTrafficData = abuseEvaluationObject.FlaggedSubdomainTrafficData ?? new List<FlaggedTrafficData>();

            List<FlaggedTrafficData> combinedFlaggedTrafficData = flaggedTrafficData.Union(flaggedSubdomainTrafficData).ToList();

            double totalAllTraffic = flaggedTrafficData.Sum(_ => _.Alltraffic);
            double totalAllSundomainTraffic = flaggedSubdomainTrafficData.Sum(_ => _.Alltraffic);
            if (totalAllTraffic == 0 && totalAllSundomainTraffic == 0) return Task.FromResult(insights);

            double totalFlaggedTraffic = flaggedTrafficData.Sum(_ => _.FlaggedTraffic);
            double totalSubdomainFlaggedTraffic = flaggedSubdomainTrafficData.Sum(_ => _.FlaggedTraffic);

            double combiedTotalFlaggedTraffic = totalFlaggedTraffic + totalSubdomainFlaggedTraffic;

            double totalFlaggedTrafficNone = flaggedTrafficData
                .Where(_ => StringComparer.InvariantCultureIgnoreCase.Equals(_.Disposition, "none"))
                .Sum(_ => _.FlaggedTraffic);

            double totalFlaggedSubdomainTrafficNone = flaggedSubdomainTrafficData
                .Where(_ => StringComparer.InvariantCultureIgnoreCase.Equals(_.Disposition, "none"))
                .Sum(_ => _.FlaggedTraffic);

            double combinedTotalFlaggedTrafficNone = totalFlaggedTrafficNone + totalFlaggedSubdomainTrafficNone;

            string urlPrefix = abuseEvaluationObject.Url;
            string startDate = abuseEvaluationObject.StartDate.ToString("yyyy-MM-dd");
            string endDate = abuseEvaluationObject.EndDate.ToString("yyyy-MM-dd");
            string domain = abuseEvaluationObject.Domain;

            if (combiedTotalFlaggedTraffic > 0)
            {
                NamedAdvisory message = new NamedAdvisory(
                    AbuseGuid,
                    "mailcheck.insights.abuseData",
                    GetType(combinedTotalFlaggedTrafficNone),
                    string.Format(AbuseInsightRulesResources.CombinedAbuseRateWithNumberDelivered,
                        combiedTotalFlaggedTraffic, abuseEvaluationObject.Domain, combinedTotalFlaggedTrafficNone),
                    string.Format(AbuseInsightRulesMarkdownResources.AbuseRateMessage,
                        domain, totalFlaggedTraffic, totalFlaggedTrafficNone, totalSubdomainFlaggedTraffic, totalFlaggedSubdomainTrafficNone, urlPrefix, startDate, endDate));

                insights.Add(message);
            }
            else if(combiedTotalFlaggedTraffic == 0)
            {
                NamedAdvisory message = new NamedAdvisory(
                    AbuseGuid,
                    "mailcheck.insights.abuseData",
                    GetType(combinedTotalFlaggedTrafficNone),
                    string.Format(AbuseInsightRulesResources.CombinedAbuseRateWithoutNumberDelivered,
                        combiedTotalFlaggedTraffic, abuseEvaluationObject.Domain),
                    string.Format(AbuseInsightRulesMarkdownResources.AbuseRateMessage, 
                        domain, totalFlaggedTraffic, totalFlaggedTrafficNone, totalSubdomainFlaggedTraffic, totalFlaggedSubdomainTrafficNone, urlPrefix, startDate, endDate));

                insights.Add(message);
            }

            return Task.FromResult(insights);
        }

        private string GetName(double noneCount)
        {
            if (noneCount < 50)
            {
                return "mailcheck.insights.infoAbuse";
            }

            if (noneCount >= 50 && noneCount <= 500)
            {
                return "mailcheck.insights.warningAbuse";
            }

            return "mailcheck.insights.errorAbuse";
        }

        private MessageType GetType(double noneCount)
        {
            if (noneCount < 50)
            {
                return MessageType.info;
            }

            if (noneCount >= 50 && noneCount <= 500)
            {
                return MessageType.warning;
            }

            return MessageType.error;
        }
    }
}