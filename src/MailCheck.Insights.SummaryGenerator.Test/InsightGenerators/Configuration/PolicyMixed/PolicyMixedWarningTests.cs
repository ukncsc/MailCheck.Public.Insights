using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyMixed;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyMixed
{
    [TestFixture]
    public class PolicyMixedWarningTests
    {
        private IConfigurationTableFactory _configurationTableFactory;
        private PolicyMixedWarning _policyMixedWarning;

        [SetUp]
        public void SetUp()
        {
            _configurationTableFactory = A.Fake<IConfigurationTableFactory>();
            _policyMixedWarning = new PolicyMixedWarning(_configurationTableFactory);
        }

        private ConfigurationEvaluationObject GetConfigurationEvaluationObject()
        {
            return new ConfigurationEvaluationObject
            {
                PercentNone = 50,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "badProvider", PercentDmarcFail = 2.1M},
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
                }
            };
        }

        [Test]
        public async Task EvaluateReturnsAdvisory()
        {
            ConfigurationEvaluationObject source = GetConfigurationEvaluationObject();

            A.CallTo(() => _configurationTableFactory.Create(
                A<List<ProviderCalculation>>.That.Matches(x => x.Exists(y => y.Name == "badProvider")),
                A<List<string>>.That.Matches(x => x.Contains("goodProvider")),
                source)).Returns("configurationMarkdown");

            List<AdvisoryMessage> result = await _policyMixedWarning.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("You are moving (or have moved recently) to a stronger DMARC policy, but you still have SPF/DKIM configuration issues with 1 sending systems", result[0].Text);
            Assert.AreEqual("configurationMarkdown", result[0].MarkDown);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateReturnsAdvisoryIfAnyMixedPctQr()
        {
            ConfigurationEvaluationObject source = GetConfigurationEvaluationObject();
            source.PercentNone = 0;
            source.TotalMixedQuarantineOrReject = 1;

            List<AdvisoryMessage> result = await _policyMixedWarning.Evaluate(source);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooHigh()
        {
            ConfigurationEvaluationObject source = GetConfigurationEvaluationObject();
            source.PercentNone = 99.1M;

            List<AdvisoryMessage> result = await _policyMixedWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooLow()
        {
            ConfigurationEvaluationObject source = GetConfigurationEvaluationObject();
            source.PercentNone = 0.9M;

            List<AdvisoryMessage> result = await _policyMixedWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfNoProvidersOverFailThreshold()
        {
            ConfigurationEvaluationObject source = GetConfigurationEvaluationObject();
            source.ProviderCalculations = new List<ProviderCalculation>
            {
                new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
            };

            List<AdvisoryMessage> result = await _policyMixedWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }
    }
}
