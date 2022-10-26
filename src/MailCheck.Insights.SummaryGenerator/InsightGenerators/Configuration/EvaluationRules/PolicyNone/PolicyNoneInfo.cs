using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyNone
{
    public class PolicyNoneInfo : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _goToQuarantineGuid = new Guid("d556f0b9-1c9c-49df-b57a-56f98ad7d41c");
        
        public int SequenceNo => 3;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            if (source.PercentNone > 99 && source.PercentFailingDmarc < 10)
            {
                decimal totalPctPassedDmarc = 100 - Math.Round(source.PercentFailingDmarc, 2);

                string title = string.Format(PolicyNoneResources.PolicyNoneInfoText, totalPctPassedDmarc);
                string markdown = string.Format(PolicyNoneResources.PolicyNoneInfoMarkdown, totalPctPassedDmarc, source.Domain, source.StartDate, source.EndDate, source.Url);

                NamedAdvisory insight = new NamedAdvisory(_goToQuarantineGuid, "mailcheck.insights.goToQuarantine", MessageType.info, title, markdown);
                insights.Add(insight);
            }

            return Task.FromResult(insights);
        }
    }
}