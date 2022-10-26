using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Subdomains;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules
{
    public class DispositionNone : IRule<SubdomainsEvaluationObject>
    {
        private readonly ISubdomainInsightAdvisoryMessageGenerator _advisoryMessageGenerator;

        private Guid Id = new Guid("3765F4F2-EB95-4ABF-B97F-9D48473B8352");

        public DispositionNone(ISubdomainInsightAdvisoryMessageGenerator advisoryMessageGenerator)
        {
            _advisoryMessageGenerator = advisoryMessageGenerator;
        }

        public int SequenceNo => 40;
        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(SubdomainsEvaluationObject subdomains)
        {
            List<AdvisoryMessage> advisoryMessages = new List<AdvisoryMessage>();

            List<SubdomainsData> result = subdomains.Data.Where(Filter)
                .OrderByDescending(x => x.AllTrafficCount).ToList();

            if (result.Count > 0)
            {
                NamedAdvisory message = _advisoryMessageGenerator.Generate(
                    Id,
                    "mailcheck.insights.dispositionNoneSubdomains",
                    MessageType.warning,
                    string.Format(SubdomainsRulesResource.DispositionNoneRule, result.Count, subdomains.Domain),
                    SubdomainsRulesMarkdownResource.DispositionNone,
                    result, Calculate);

                advisoryMessages.Add(message);
            }

            return Task.FromResult(advisoryMessages);
        }

        private bool Filter(SubdomainsData subdomainsData)
        {
            return subdomainsData.AllTrafficCount > 10 &&  subdomainsData.Policy == "none";
        }

        private int Calculate(SubdomainsData subdomainsData)
        {
            return (subdomainsData.DmarcFailCount * 100) / subdomainsData.AllTrafficCount;
        }
    }
}