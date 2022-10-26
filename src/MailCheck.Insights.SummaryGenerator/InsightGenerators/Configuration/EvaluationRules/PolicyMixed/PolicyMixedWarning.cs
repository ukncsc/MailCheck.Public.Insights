using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyMixed
{
    public class PolicyMixedWarning : IRule<ConfigurationEvaluationObject>
    {
        private readonly Guid _policyMixedMessageGuid = new Guid("9638ba40-3090-4bba-b1fd-9d2007d81e13");
        private readonly IConfigurationTableFactory _configurationTableFactory;

        public PolicyMixedWarning(IConfigurationTableFactory configurationTableFactory)
        {
            _configurationTableFactory = configurationTableFactory;
        }

        public int SequenceNo => 2;

        public bool IsStopRule => false;

        public Task<List<AdvisoryMessage>> Evaluate(ConfigurationEvaluationObject source)
        {
            List<AdvisoryMessage> insights = new List<AdvisoryMessage>();

            bool policiesMixed = source.PercentNone >= 1 && source.PercentNone <= 99 || source.TotalMixedQuarantineOrReject > 0;
            bool providersWithDmarcFailures = source.ProviderCalculations != null && source.ProviderCalculations.Any(x => x.PercentDmarcFail > 2);

            if (policiesMixed && providersWithDmarcFailures)
            {
                List<string> goodProviders = new List<string>();
                List<ProviderCalculation> badProviders = new List<ProviderCalculation>();

                foreach (ProviderCalculation providerData in source.ProviderCalculations)
                {
                    if (providerData.PercentDmarcFail <= 2)
                    {
                        goodProviders.Add(providerData.Name);
                    }
                    else
                    {
                        badProviders.Add(providerData);
                    }
                }

                if (badProviders.Count == 0) return Task.FromResult(insights);

                string title = string.Format(PolicyMixedResources.PolicyMixedWarningTitle, badProviders.Count);
                string markdown = _configurationTableFactory.Create(badProviders, goodProviders, source);

                NamedAdvisory insight = new NamedAdvisory(_policyMixedMessageGuid, "mailcheck.insights.policyMixedWarning", MessageType.warning, title, markdown);
                insights.Add(insight);
            }
            return Task.FromResult(insights);
        }
    }
}
