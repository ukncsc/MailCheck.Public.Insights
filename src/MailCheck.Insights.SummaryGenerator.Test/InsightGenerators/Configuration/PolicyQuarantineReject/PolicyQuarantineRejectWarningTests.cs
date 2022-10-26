using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyQuarantineReject;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyQuarantineReject
{
    [TestFixture]
    public class PolicyQuarantineRejectWarningTests
    {
        private PolicyQuarantineRejectWarning _policyQuarantineRejectWarning;

        [SetUp]
        public void SetUp()
        {
            _policyQuarantineRejectWarning = new PolicyQuarantineRejectWarning();
        }

        [Test]
        public async Task EvaluateReturnsAdvisory()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentQuarantineOrReject = 99.1M,
                StartDate = new DateTime(2001, 01, 01),
                EndDate = new DateTime(2002, 02, 02),
                Domain = "testDomain",
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "badProvider", PercentDmarcFail = 2.1M}
                }
            };

            List<AdvisoryMessage> result = await _policyQuarantineRejectWarning.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mail Check has detected 1 system(s) that might be legitimate for you to review that is failing SPF and DKIM, and therefore being sent to Junk or blocked altogether", result[0].Text);
            Assert.AreEqual($"You have a DMARC policy of quarantine or reject, which means emails from testDomain that fail SPF and DKIM will be sent to Junk or blocked altogether.{Environment.NewLine}{Environment.NewLine}However, Mail Check detected 1 system(s) that might be legitimate that are failing SPF and DKIM:{Environment.NewLine}{Environment.NewLine}badProvider{Environment.NewLine}{Environment.NewLine}[Go to Email Traffic page for testDomain](/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic){Environment.NewLine}{Environment.NewLine}You either need to configure the system(s) with SPF and/or DKIM, or if the system is not approved for use in your organisation, you will need to notify the relevant team to use an approved alternative.", result[0].MarkDown);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryPercentQuarantineRejectTooLow()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentQuarantineOrReject = 99,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
                }
            };

            List<AdvisoryMessage> result = await _policyQuarantineRejectWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfNoProviderOverFailThreshold()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentQuarantineOrReject = 99.1M,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "badProvider", PercentDmarcFail = 2}
                }
            };

            List<AdvisoryMessage> result = await _policyQuarantineRejectWarning.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }
    }
}
