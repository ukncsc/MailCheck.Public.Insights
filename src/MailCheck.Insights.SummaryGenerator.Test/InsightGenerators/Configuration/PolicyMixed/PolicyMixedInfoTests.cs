using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyMixed;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyMixed
{
    [TestFixture]
    public class PolicyMixedInfoTests
    {
        private PolicyMixedInfo _policyMixedInfo;

        [SetUp]
        public void SetUp()
        {
            _policyMixedInfo = new PolicyMixedInfo();
        }

        [Test]
        public async Task EvaluateReturnsAdvisory()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();

            List<AdvisoryMessage> result = await _policyMixedInfo.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("You are moving (or have moved recently) to a stronger DMARC policy, and Mail Check did not find any email sending systems that need configuring with SPF/ DKIM.", result[0].Text);
            Assert.AreEqual($"Looking at your DMARC reporting in the last 30 days, 91% of your emails are passing SPF and DKIM checks.{Environment.NewLine}{Environment.NewLine}Note that Mail Check only detects systems to configure that contribute > 10 emails and > 1% of your traffic per month. You can [review your DMARC reporting](testUrl/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic) to check if there are further lower volumes systems that might need configuring.{Environment.NewLine}{Environment.NewLine}For further information on how to implement this change, [refer to our guidance](https://www.ncsc.gov.uk/collection/email-security-and-anti-spoofing/mark-spoof-emails-as-spam) on how to move to a policy of 'quarantine'.", result[0].MarkDown);
            Assert.AreEqual(MessageType.info, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateReturnsAdvisoryIfAnyMixedPctQr()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();
            source.PercentNone = 0;
            source.TotalMixedQuarantineOrReject = 1;

            List<AdvisoryMessage> result = await _policyMixedInfo.Evaluate(source);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooLow()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();
            source.PercentNone = 0.9M;

            List<AdvisoryMessage> result = await _policyMixedInfo.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooHigh()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();
            source.PercentNone = 99.1M;

            List<AdvisoryMessage> result = await _policyMixedInfo.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfAnyProvidersOverFailThreshold()
        {
            ConfigurationEvaluationObject source = GetTestConfigurationEvaluationObject();
            source.ProviderCalculations[0].PercentDmarcFail = 2.1M;

            List<AdvisoryMessage> result = await _policyMixedInfo.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        private ConfigurationEvaluationObject GetTestConfigurationEvaluationObject()
        {
            return new ConfigurationEvaluationObject
            {
                PercentNone = 50,
                PercentFailingDmarc = 9,
                StartDate = new DateTime(2001, 01, 01),
                EndDate = new DateTime(2002, 02, 02),
                Domain = "testDomain",
                Url = "testUrl",
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
                }
            };
        }
    }
}
