using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyNone;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration.PolicyNone
{
    [TestFixture]
    public class PolicyNoneInfoTests
    {
        private PolicyNoneInfo _policyNoneInfo;

        [SetUp]
        public void SetUp()
        {
            _policyNoneInfo = new PolicyNoneInfo();
        }

        [Test]
        public async Task EvaluateReturnsAdvisory()
        {
            ConfigurationEvaluationObject source = GetTestEvaluationObject();
            List<AdvisoryMessage> result = await _policyNoneInfo.Evaluate(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mail Check has identified that 91% of your emails are passing DMARC, so you are ready (or nearly ready) to upgrade your DMARC policy to 'quarantine'", result[0].Text);
            Assert.AreEqual($"Looking at your DMARC reporting in the last 30 days, 91% of your emails are passing DMARC. Note this applies to emails from testDomain but not sub-domains (see separate DMARC Insights).{Environment.NewLine}{Environment.NewLine}**You should start considering how and when you will move to a DMARC policy of ‘Quarantine’**{Environment.NewLine}{Environment.NewLine}A DMARC policy of quarantine will ensure that malicious spoofed emails that fail SPF and DKIM checks are sent to Junk folders.{Environment.NewLine}{Environment.NewLine}You should consider the following:{Environment.NewLine}{Environment.NewLine}- Are all your email sending systems* on testDomain configured with SPF and/or DKIM?{Environment.NewLine}  - If yes, then you can proceed with moving your policy to ‘quarantine’{Environment.NewLine}  - If no, then continue to configure SPF and DKIM first before moving to a policy of quarantine{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}- Are your sub-domain traffic passing DMARC and ready for a quarantine policy too?{Environment.NewLine}  - If yes, then you can apply a policy of quarantine (p=quarantine) that applies to testDomain and any sub-domains{Environment.NewLine}  - If no, then you should keep your sub-domains on a policy of ‘none’. To do this you will need to include both ‘p=quarantine’ and ‘sp=none’ in your policy, where sp=none keeps your sub-domains with a DMARC policy of none for the time being.{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}- Have you read [our guidance](https://www.ncsc.gov.uk/collection/email-security-and-anti-spoofing/mark-spoof-emails-as-spam)? This covers considerations about how to engage stakeholders and ‘dial up’ to a full quarantine policy.{Environment.NewLine}  {Environment.NewLine}{Environment.NewLine}***Note:** DMARC Insights only highlights systems that contribute > 10 emails and > 1% of your traffic per month, so you should consider checking [your DMARC reporting](testUrl/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic) if there are lower volume systems that still might need to be configured.", result[0].MarkDown);
            Assert.AreEqual(MessageType.info, result[0].MessageType);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfAboveFailThreshold()
        {
            ConfigurationEvaluationObject source = GetTestEvaluationObject();
            source.PercentFailingDmarc = 10;

            List<AdvisoryMessage> result = await _policyNoneInfo.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task EvaluateDoesNotReturnAdvisoryIfPercentNoneTooLow()
        {
            ConfigurationEvaluationObject source = GetTestEvaluationObject();
            source.PercentNone = 99;

            List<AdvisoryMessage> result = await _policyNoneInfo.Evaluate(source);

            Assert.AreEqual(0, result.Count);
        }

        private ConfigurationEvaluationObject GetTestEvaluationObject()
        {
            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                PercentNone = 99.1M,
                PercentFailingDmarc = 9,
                StartDate = new DateTime(2001, 01, 01),
                EndDate = new DateTime(2002, 02, 02),
                Domain = "testDomain",
                Url = "testUrl"
            };

            return source;
        }
    }
}
