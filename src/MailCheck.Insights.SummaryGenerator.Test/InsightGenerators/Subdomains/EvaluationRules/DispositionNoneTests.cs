using System;
using System.Collections.Generic;
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
    public class DispositionNoneTests
    {
        private DispositionNone _rule;
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
            _rule = new DispositionNone(_advisoryMessageGenerator);
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
                        Domain = "test1.abc.gov.uk",
                        DmarcFailCount = 10,
                        Policy="reject"
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
                        AllTrafficCount = 80,
                        DmarcFailCount = 10,
                        Policy="none"
                    }
                });

            string expectedMarkdown =
                "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test1.abc.gov.uk|80|12%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test1.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days";


            string expectedTitle = "In the last 30 days, Mail Check saw 1 subdomains of abc.gov.uk that had a DMARC policy of 'none'. Some or all of these have SPF and DKIM configuration issues.";

            List<AdvisoryMessage> result = await
                _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        [Test]
        public async Task NoPolicyNoneRecord()
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
                    }
                });

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(0, result.Count);
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
                        AllTrafficCount = 80,
                        DmarcFailCount = 10,
                        Policy= "none"
                    },
                     new SubdomainsData
                    {
                        Domain = "test2.abc.gov.uk",
                        AllTrafficCount = 15,
                        DmarcFailCount = 15,
                         Policy= "none"
                    },
                     new SubdomainsData
                    {
                        Domain = "test3.abc.gov.uk",
                        AllTrafficCount = 130,
                        DmarcFailCount = 19,
                         Policy= "none"
                    }
                });

            string expectedMarkdown = "| Subdomain | Email | % failing DMARC | Link |" + NewLine() + "|---|---|---|---|" + NewLine() +
                "|test3.abc.gov.uk|130|14%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test3.abc.gov.uk/dmarc-insights)|" + NewLine() +
                "|test1.abc.gov.uk|80|12%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test1.abc.gov.uk/dmarc-insights)|" + NewLine() +
                "|test2.abc.gov.uk|15|100%|[View](https://www.mailcheck.service.ncsc.gov.uk/app/domain-security/test2.abc.gov.uk/dmarc-insights)|" + NewLine() + "---" + NewLine() + NewLine() +
                "**Note:** This list only contains subdomains with > 10 emails in the last 30 days";


            string expectedTitle =
                "In the last 30 days, Mail Check saw 3 subdomains of abc.gov.uk that had a DMARC policy of 'none'. Some or all of these have SPF and DKIM configuration issues.";

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedMarkdown, result[0].MarkDown);
            Assert.AreEqual(expectedTitle, result[0].Text);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        private string NewLine()
        {
            return Environment.NewLine;
        }
    }
}