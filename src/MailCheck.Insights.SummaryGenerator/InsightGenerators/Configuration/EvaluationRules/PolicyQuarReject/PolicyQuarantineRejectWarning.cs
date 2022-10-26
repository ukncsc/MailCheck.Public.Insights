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
    public class PolicyQuarantineRejectWarning : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _policyQuarantineRejectWarning = new Guid("98f52930-6806-4661-8191-ce7ce7fd2f2e");

        public int SequenceNo => 6;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            bool providersWithDmarcFailures = source.ProviderCalculations != null && source.ProviderCalculations.Any(x => x.PercentDmarcFail > 2);

            if (source.PercentQuarantineOrReject > 99 && providersWithDmarcFailures && source.TotalMixedQuarantineOrReject == 0)
            {
                List<string> badProviders = source.ProviderCalculations
                    .Where(x => x.PercentDmarcFail > 2)
                    .Select(x => x.Name)
                    .ToList();

                string text = string.Format(PolicyQuarRejectRes.PolicyQrWarningText, badProviders.Count);
                string markdown = string.Format(PolicyQuarRejectRes.PolicyQrWarningMarkdown, source.Domain, badProviders.Count, string.Join(Environment.NewLine, badProviders), source.StartDate, source.EndDate, source.Url);

                NamedAdvisory insight = new NamedAdvisory(_policyQuarantineRejectWarning, "mailcheck.insights.configQuarRejectWarning", MessageType.warning, text, markdown);
                insights.Add(insight);
            }

            return Task.FromResult(insights);
        }
    }
}
