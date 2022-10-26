using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyNone
{
    public class PolicyNoneWarning : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _policyNoneMessageGuid = new Guid("3175d33b-735c-4671-84b2-9bb2a2d98983");
        private readonly IConfigurationTableFactory _configurationTableFactory;

        public PolicyNoneWarning(IConfigurationTableFactory configurationTableFactory)
        {
            _configurationTableFactory = configurationTableFactory;
        }

        public int SequenceNo => 4;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            if (source.PercentNone > 99)
            {
                List<string> goodProviders = new List<string>();
                List<ProviderCalculation> badProviders = new List<ProviderCalculation>();

                foreach (ProviderCalculation providerData in source.ProviderCalculations)
                {
                    if (providerData.PercentDmarcFail < 2)
                    {
                        goodProviders.Add(providerData.Name);
                    }
                    else
                    {
                        badProviders.Add(providerData);
                    }
                }

                if (badProviders.Count == 0) return Task.FromResult(insights);

                string title = string.Format(PolicyNoneResources.PolicyNoneWarningTitle, Math.Round(source.PercentFailingDmarc, 2), badProviders.Count);
                string markdown = _configurationTableFactory.Create(badProviders, goodProviders, source);

                NamedAdvisory insight = new NamedAdvisory(_policyNoneMessageGuid, "mailcheck.insights.providerConfigNone", MessageType.warning, title, markdown);
                insights.Add(insight);
            }

            return Task.FromResult(insights);
        }
    }
}