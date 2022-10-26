using System;
using System.Collections.Generic;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration
{
    [TestFixture]
    public class ConfigurationTableFactoryTests
    {
        private ConfigurationTableFactory _policyMixedInfo;

        [SetUp]
        public void SetUp()
        {
            _policyMixedInfo = new ConfigurationTableFactory();
        }

        [Test]
        public void EvaluateReturnsAdvisory()
        {
            List<ProviderCalculation> badProviders = new List<ProviderCalculation>()
            {
                new ProviderCalculation {
                    Name = "badProvider",
                    PercentDmarcFail = 1.1M,
                    PercentSpfAuthFail = 2.2M,
                    PercentSpfMisaligned = 3.3M,
                    PercentDkimAuthFail = 4.4M,
                    PercentDkimMisaligned = 5.5M
                }
            };

            List<string> goodProviders = new List<string> { "goodProvider" };

            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                StartDate = new DateTime(2001, 01, 01),
                EndDate = new DateTime(2002, 02, 02),
                Domain = "testDomain"
            };

            string result = _policyMixedInfo.Create(badProviders, goodProviders, source);

            Assert.AreEqual($"### Well configured systems that are passing DMARC.{Environment.NewLine}{Environment.NewLine}- goodProvider{Environment.NewLine}{Environment.NewLine}---{Environment.NewLine}### Systems that need configuring with SPF or DKIM:{Environment.NewLine}**Note.** The table below only shows providers that contribute > 10 emails and > 1% of overall traffic volume per month. You should [review your DMARC reporting](/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic) to check if there are further lower volume systems that need configuring.{Environment.NewLine}{Environment.NewLine}| Provider | Dmarc Fail | SPF Auth Fail | SPF Misaligned | DKIM Auth Fail | DKIM Misaligned |{Environment.NewLine}|---|---|---|---|---|---|{Environment.NewLine}|badProvider|1%|2%|3%|4%|6%|{Environment.NewLine}---{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{ Environment.NewLine}{Environment.NewLine}### Understanding this table{Environment.NewLine}- **DMARC fail.** % emails that are failing DMARC ie failing both SPF and DKIM. To pass SPF overall, your emails will need to pass both an authentication and an alignment check. The same is true for DKIM.{Environment.NewLine}- **Auth fail.**  % emails failing authentication. For SPF, the receiver checks your SPF record to see that the sending IP address is valid. For DKIM, the receiver checks the validity of DKIM signing.{Environment.NewLine}- **Misaligned.** % emails failing SPF or DKIM due to misalignment. An alignment check ensures that the various ‘from’ addresses on your email match the ‘header from’ address that is seen by the recipient. This addresses scenarios where attackers get malicious emails to pass authentication.{Environment.NewLine}{Environment.NewLine}**Further Information:** [Guidance on configuring SPF and DKIM](https://www.ncsc.gov.uk/collection/email-security-and-anti-spoofing/configure-anti-spoofing-controls-){Environment.NewLine}", result);
        }

        [Test]
        public void EvaluateReturnsAdvisoryContainingGoogleInformation()
        {
            List<ProviderCalculation> badProviders = new List<ProviderCalculation>()
            {
                new ProviderCalculation {
                    Name = "Google",
                    PercentDmarcFail = 1.1M,
                    PercentSpfAuthFail = 2.2M,
                    PercentSpfMisaligned = 3.3M,
                    PercentDkimAuthFail = 4.4M,
                    PercentDkimMisaligned = 5.5M
                },
                new ProviderCalculation {
                    Name = "badProviderName",
                    PercentDmarcFail = 1.1M,
                    PercentSpfAuthFail = 2.2M,
                    PercentSpfMisaligned = 3.3M,
                    PercentDkimAuthFail = 4.4M,
                    PercentDkimMisaligned = 5.5M
                }
            };

            List<string> goodProviders = new List<string> { "goodProvider" };

            ConfigurationEvaluationObject source = new ConfigurationEvaluationObject
            {
                StartDate = new DateTime(2001, 01, 01),
                EndDate = new DateTime(2002, 02, 02),
                Domain = "testDomain"
            };

            string result = _policyMixedInfo.Create(badProviders, goodProviders, source);

            Assert.AreEqual($"### Well configured systems that are passing DMARC.{Environment.NewLine}{Environment.NewLine}- goodProvider{Environment.NewLine}{Environment.NewLine}---{Environment.NewLine}### Systems that need configuring with SPF or DKIM:{Environment.NewLine}**Note.** The table below only shows providers that contribute > 10 emails and > 1% of overall traffic volume per month. You should [review your DMARC reporting](/app/domain-security/testDomain/custom/2001-01-01/2002-02-02/false/emailtraffic) to check if there are further lower volume systems that need configuring.{Environment.NewLine}{Environment.NewLine}| Provider | Dmarc Fail | SPF Auth Fail | SPF Misaligned | DKIM Auth Fail | DKIM Misaligned |{Environment.NewLine}|---|---|---|---|---|---|{Environment.NewLine}|Google *|1%|2%|3%|4%|6%|{Environment.NewLine}|badProviderName|1%|2%|3%|4%|6%|{Environment.NewLine}---{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}\\* If you don't use Google, this traffic is likely to be auto-forwarding via Gmail etc. Configure DKIM on your email senders to reduce the negative effects of auto-forwarding.{Environment.NewLine}{Environment.NewLine}### Understanding this table{Environment.NewLine}- **DMARC fail.** % emails that are failing DMARC ie failing both SPF and DKIM. To pass SPF overall, your emails will need to pass both an authentication and an alignment check. The same is true for DKIM.{Environment.NewLine}- **Auth fail.**  % emails failing authentication. For SPF, the receiver checks your SPF record to see that the sending IP address is valid. For DKIM, the receiver checks the validity of DKIM signing.{Environment.NewLine}- **Misaligned.** % emails failing SPF or DKIM due to misalignment. An alignment check ensures that the various ‘from’ addresses on your email match the ‘header from’ address that is seen by the recipient. This addresses scenarios where attackers get malicious emails to pass authentication.{Environment.NewLine}{Environment.NewLine}**Further Information:** [Guidance on configuring SPF and DKIM](https://www.ncsc.gov.uk/collection/email-security-and-anti-spoofing/configure-anti-spoofing-controls-){Environment.NewLine}", result);
        }
    }
}
