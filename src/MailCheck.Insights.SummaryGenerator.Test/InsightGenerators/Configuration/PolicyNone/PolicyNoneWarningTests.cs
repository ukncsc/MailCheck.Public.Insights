using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyNone;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyNone
{
    [TestFixture]
    public class PolicyNoneWarningTests
    {
        private PolicyNoneWarning _policyNoneWarning;
        private IConfigurationTableFactory _configurationTableFactory;

        [SetUp]
        public void SetUp()
        {
            _configurationTableFactory = A.Fake<IConfigurationTableFactory>();
            _policyNoneWarning = new PolicyNoneWarning(_configurationTableFactory);
        }

        [Test]
        public async Task EvaluateReturnsAdvisory()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();

            A.CallTo(() => _configurationTableFactory.Create(
                A<List<ProviderCalculation>>.That.Matches(x => x.Exists(y => y.Name == "badProvider")),
                A<List<string>>.That.Matches(x => x.Contains("goodProvider")),
                source)).Returns("configurationMarkdown");

            List<AdvisoryMessage> result = await _policyNoneWarning.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mail Check identified that 9% of your email traffic is failing SPF and DKIM checks, related to configuration issues with 1 email sending system(s)", result[0].Text);
            Assert.AreEqual("configurationMarkdown", result[0].MarkDown);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooLow()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();
            source.PercentNone = 99;

            List<AdvisoryMessage> result = await _policyNoneWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfAllProvidersBelowFailThreshold()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();

            source.ProviderCalculations = new List<ProviderCalculation>
            {
                new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 1.9M}
            };

            List<AdvisoryMessage> result = await _policyNoneWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        private ConfigurationEvaluationObject GetTestConfigurationEvaluationObject()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentNone = 99.1M,
                PercentFailingDmarc = 9,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "badProvider", PercentDmarcFail = 2},
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 1.9M}
                }
            };

            return source;
        }
    }
}
