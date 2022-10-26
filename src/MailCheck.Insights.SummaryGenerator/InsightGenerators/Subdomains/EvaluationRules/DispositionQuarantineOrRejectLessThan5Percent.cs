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
    public class DispositionQuarantineOrRejectLessThan5Percent : IRule<SubdomainsEvaluationObject>
    {
        private readonly ISubdomainInsightAdvisoryMessageGenerator _advisoryMessageGenerator;

        private Guid Id = new Guid("C03A0968-598C-45C0-AC10-B0940DD44A16");

        public DispositionQuarantineOrRejectLessThan5Percent(ISubdomainInsightAdvisoryMessageGenerator advisoryMessageGenerator)
        {
            _advisoryMessageGenerator = advisoryMessageGenerator;
        }

        public int SequenceNo => 41;
        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(SubdomainsEvaluationObject subdomains)
        {
            List<AdvisoryMessage> advisoryMessages = new List<AdvisoryMessage>();

            List<SubdomainsData> result = subdomains.Data.Where(RejectOrQuarantineLessThan5Percent)
                .OrderByDescending(x => x.AllTrafficCount).ToList();

            if (result.Count > 0)
            {
                NamedAdvisory message = _advisoryMessageGenerator.Generate(
                    Id,
                    "mailcheck.insights.dispositionQuarRejectLessThan5",
                    MessageType.success,
                    string.Format(SubdomainsRulesResource.DispositionQuarantineOrRejectLessThan5Percent, result.Count,
                        subdomains.Domain),
                    SubdomainsRulesMarkdownResource.DispositionQuarantineOrRejectLessThan5Percent,
                    result, Calculate);

                advisoryMessages.Add(message);
            }

            return Task.FromResult(advisoryMessages);
        }

        private bool RejectOrQuarantineLessThan5Percent(SubdomainsData subdomainsData)
        {
            if (subdomainsData.AllTrafficCount > 10 && (subdomainsData.Policy == "reject" || subdomainsData.Policy == "quarantine"))
            {
                return ((subdomainsData.DmarcFailCount * 100) /
                        subdomainsData.AllTrafficCount) < 5;
            }

            return false;
        }

        private int Calculate(SubdomainsData subdomainsData)
        {
            return (subdomainsData.DmarcFailCount * 100) / subdomainsData.AllTrafficCount;
        }
    }
}
