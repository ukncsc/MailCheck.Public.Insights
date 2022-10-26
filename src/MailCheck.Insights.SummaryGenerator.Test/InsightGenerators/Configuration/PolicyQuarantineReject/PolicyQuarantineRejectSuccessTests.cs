using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyQuarantineReject;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyQuarantineReject
{
    [TestFixture]
    public class PolicyQuarantineRejectSuccessTests
    {
        private PolicyQuarantineRejectSuccess _policyQuarantineRejectSuccess;

        [SetUp]
        public void SetUp()
        {
            _policyQuarantineRejectSuccess = new PolicyQuarantineRejectSuccess();
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
                LatestPolicy = "reject",
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
                }
            };

            List<AdvisoryMessage> result = await _policyQuarantineRejectSuccess.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mail Check has not detected any systems that need configuring with SPF and DKIM", result[0].Text);
            Assert.AreEqual($"Mail Check has not detected any systems that need configuring with SPF and DKIM{Environment.NewLine}{Environment.NewLine}Mail Check won't have detected any systems with low volumes, contributing < 10 emails or < 1% of your overall email traffic per month.{Environment.NewLine}{Environment.NewLine}You may want to review your DMARC reporting to check if there are any further systems that need configuring.{Environment.NewLine}{Environment.NewLine}[Review email traffic marked as rejected](/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic?categoryFilter=rejected)", result[0].MarkDown);
            Assert.AreEqual(MessageType.success, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentQuarantineRejectTooLow()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentQuarantineOrReject = 99,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "goodProvider", PercentDmarcFail = 2}
                }
            };

            List<AdvisoryMessage> result = await _policyQuarantineRejectSuccess.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfAnyProviderOverFailThreshold()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentQuarantineOrReject = 99.1M,
                ProviderCalculations = new List<ProviderCalculation>
                {
                    new ProviderCalculation {Name = "badProvider", PercentDmarcFail = 2.1M}
                }
            };
            
            List<AdvisoryMessage> result = await _policyQuarantineRejectSuccess.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }
    }
}
