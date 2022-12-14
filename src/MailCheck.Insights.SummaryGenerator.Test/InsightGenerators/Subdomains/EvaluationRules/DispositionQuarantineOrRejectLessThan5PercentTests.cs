using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.Contracts.Subdomains;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Subdomains.EvaluationRules
{
    [TestFixture]
    public class DispositionQuarantineOrRejectLessThan5PercentTests
    {
        private DispositionQuarantineOrRejectLessThan5Percent _rule;
        private ISubdomainInsightAdvisoryMessageGenerator _advisoryMessageGenerator;
        private static Guid Id = new Guid("3175d33b-735c-4671-84b2-9bb2a2d98983");
        private ISummaryGeneratorConfig _config;
        private string MailCheckUrlPrefix = "https://www.mailcheck.service.ncsc.gov.uk";
        [SetUp]
        public void Setup()
        {
            _config = A.Fake<ISummaryGeneratorConfig>();
            A.CallTo(() => _config.MailCheckUrlPrefix).Returns(MailCheckUrlPrefix);
            _advisoryMessageGenerator = new SubdomainInsightAdvisoryMessageGenerator(_config);
            _rule = new DispositionQuarantineOrRejectLessThan5Percent(_advisoryMessageGenerator);
        }

        [Test]
        public async Task NoData()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk");

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task NoRecordMatch()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk"
                    }
                });

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task NoQuarantineOrRejectRecord()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 80,
                        DmarcFailCount = 10,
                        Policy= "none"
                    }
                });

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task SingleRecord()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 90,
                        DmarcFailCount = 2,
                        Policy= "reject"
                    }
                });

            string expectedMarkdown = "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test1.abc.gov.uk|90|2%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test1.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days, with <5% failing DMARC";

            string expectedTitle =
                "In the last 30 days, Mail Check saw 1 subdomains of abc.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.";

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.success, result[0].MessageType);
        }

        [Test]
        public async Task RecordsMoreThan5PercentIgnored()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 80,
                        DmarcFailCount = 10,
                        Policy= "reject"
                    },
                     new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 90,
                        DmarcFailCount = 2,
                        Policy= "quarantine"
                    }
                });

            string expectedMarkdown = "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test1.abc.gov.uk|90|2%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test1.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days, with <5% failing DMARC";

            string expectedTitle =
                "In the last 30 days, Mail Check saw 1 subdomains of abc.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.";

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.success, result[0].MessageType);
        }

        [Test]
        public async Task MultipleRecords()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 100,
                        DmarcFailCount = 3,
                        Policy= "reject"
                    },
                     new SubdomainsData
                    {
                        Domain = "test2.abc.gov.uk",
                        AllTrafficCount = 100,
                        DmarcFailCount = 2,
                        Policy= "quarantine"
                    },
                     new SubdomainsData
                    {
                        Domain = "test3.abc.gov.uk",
                        AllTrafficCount = 130,
                        DmarcFailCount = 6,
                         Policy= "reject"
                    }
                });

            string expectedMarkdown = "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test3.abc.gov.uk|130|4%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test3.abc.gov.uk/dmarc-insights)|" + NewLine() +
                "|test1.abc.gov.uk|100|3%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test1.abc.gov.uk/dmarc-insights)|" + NewLine() +
                "|test2.abc.gov.uk|100|2%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test2.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days, with <5% failing DMARC";

            string expectedTitle =
                "In the last 30 days, Mail Check saw 3 subdomains of abc.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.";

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.success, result[0].MessageType);
        }

        [Test]
        public async Task RecordsWithLessThan10EmailsIgnored()
        {
            SubdomainsEvaluationObject testData = new SubdomainsEvaluationObject("abc.gov.uk",
                new List<SubdomainsData>
                {
                    new SubdomainsData
                    {
                        Domain = "test1.abc.gov.uk",
                        AllTrafficCount = 6,
                        DmarcFailCount = 0,
                        Policy= "quarantine"
                    },
                     new SubdomainsData
                    {
                        Domain = "test2.abc.gov.uk",
                        AllTrafficCount = 5,
                        DmarcFailCount = 0,
                        Policy= "reject"
                    },
                     new SubdomainsData
                    {
                        Domain = "test3.abc.gov.uk",
                        AllTrafficCount = 11,
                        DmarcFailCount = 0,
                         Policy= "reject"
                    }
                });

            string expectedMarkdown = "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test3.abc.gov.uk|11|0%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test3.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days, with <5% failing DMARC";

            string expectedTitle =
                "In the last 30 days, Mail Check saw 1 subdomains of abc.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.";

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.success, result[0].MessageType);
        }

        private string NewLine()
        {
            return Environment.NewLine;
        }
    }
}
