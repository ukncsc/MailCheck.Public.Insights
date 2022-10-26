using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyQuarReject;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyQuarantineReject
{
    public class PolicyQuarantineRejectSuccess : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _policyQuarantineRejectSuccess = new Guid("f8dc1c5a-2dc6-46e3-87ed-b6826ac29e51");

        public int SequenceNo => 5;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            bool providersWithDmarcFailures = source.ProviderCalculations != null && source.ProviderCalculations.Any(x => x.PercentDmarcFail > 2);

            if (source.PercentQuarantineOrReject > 99 && !providersWithDmarcFailures && source.TotalMixedQuarantineOrReject == 0)
            {
                string text = PolicyQuarRejectRes.PolicyQrSuccessText;
                string filter = source.LatestPolicy == "reject" ? "rejected" : "quarantined";
                string markdown = string.Format(PolicyQuarRejectRes.PolicyQrSuccessMarkdown, source.Domain, source.StartDate.ToString("yyyy-MM-dd"), source.EndDate.ToString("yyyy-MM-dd"), filter, source.Url);

                NamedAdvisory insight = new NamedAdvisory(_policyQuarantineRejectSuccess, "mailcheck.insights.configQuarRejectSuccess", MessageType.success, text, markdown);
                insights.Add(insight);
            }
            return Task.FromResult(insights);
        }
    }
}
