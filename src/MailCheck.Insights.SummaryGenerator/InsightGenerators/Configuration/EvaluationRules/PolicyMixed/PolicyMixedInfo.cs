using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyMixed
{
    public class PolicyMixedInfo : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _policyMixedMessageGuid = new Guid("9638ba40-3090-4bba-b1fd-9d2007d81e13");
        
        public int SequenceNo => 1;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            bool policiesMixed = source.PercentNone >= 1 && source.PercentNone <= 99 || source.TotalMixedQuarantineOrReject > 0;
            bool providersWithDmarcFailures = source.ProviderCalculations != null && source.ProviderCalculations.Any(x => x.PercentDmarcFail > 2);

            if (policiesMixed && !providersWithDmarcFailures)
            {
                decimal totalPctPassedDmarc = 100 - Math.Round(source.PercentFailingDmarc, 2);
                string title = PolicyMixedResources.PolicyMixedInfoTitle;
                string markdown = string.Format(PolicyMixedResources.PolicyMixedInfoMarkdown, totalPctPassedDmarc, source.Url, source.Domain, source.StartDate, source.EndDate);

                NamedAdvisory insight = new NamedAdvisory(_policyMixedMessageGuid, "mailcheck.insights.policyMixedInfo", MessageType.info, title, markdown);
                insights.Add(insight);
            }
            return Task.FromResult(insights);
        }
    }
}
