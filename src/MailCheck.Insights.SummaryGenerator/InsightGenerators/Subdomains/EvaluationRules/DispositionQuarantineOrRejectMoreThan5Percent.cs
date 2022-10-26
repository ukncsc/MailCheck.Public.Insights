using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Subdomains;
using MailCheck.Insights.SummaryGenerator.Config;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules
{
    public class DispositionQuarantineOrRejectMoreThan5Percent : IRule<SubdomainsEvaluationObject>
    {
        private readonly ISubdomainInsightAdvisoryMessageGenerator _advisoryMessageGenerator;

        private Guid Id = new Guid("AABF942A-8547-4596-83F8-D62E435C4062");

        public DispositionQuarantineOrRejectMoreThan5Percent(ISubdomainInsightAdvisoryMessageGenerator advisoryMessageGenerator)
        {
            _advisoryMessageGenerator = advisoryMessageGenerator;
        }

        public int SequenceNo => 42;
        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(SubdomainsEvaluationObject subdomains)
        {
            List<AdvisoryMessage> advisoryMessages = new List<AdvisoryMessage>();

            List<SubdomainsData> result = subdomains.Data.Where(RejectOrQuarantineMoreThan5Percent)
                .OrderByDescending(x => x.AllTrafficCount).ToList();

            if (result.Count > 0)
            {
                NamedAdvisory message = _advisoryMessageGenerator.Generate(
                    Id,
                    "mailcheck.insights.dispositionQuarRejMoreThan5",
                    MessageType.warning,
                    string.Format(SubdomainsRulesResource.DispositionQuarantineOrRejectMoreThan5Percent, result.Count,
                        subdomains.Domain),
                    SubdomainsRulesMarkdownResource.DispositionQuarantineOrRejectMoreThan5Percent,
                    result, Calculate);

                advisoryMessages.Add(message);
            }

            return Task.FromResult(advisoryMessages);
        }

        private bool RejectOrQuarantineMoreThan5Percent(SubdomainsData subdomainsData)
        {
            if (subdomainsData.AllTrafficCount > 10 && (subdomainsData.Policy == "reject" || subdomainsData.Policy == "quarantine"))
            {
                return ((subdomainsData.DmarcFailCount * 100) /
                        subdomainsData.AllTrafficCount) > 5;
            }

            return false;
        }

        private int Calculate(SubdomainsData subdomainsData)
        {
            return (subdomainsData.DmarcFailCount * 100) / subdomainsData.AllTrafficCount;
        }
    }
}
