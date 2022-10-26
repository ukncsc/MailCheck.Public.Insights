using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Abuse
{
    [TestFixture]
    public class AbuseRateWithAllPoliciesTests
    {
        private AbuseRateWithAllPolicies _rule;

        [SetUp]
        public void Setup()
        {
            _rule = new AbuseRateWithAllPolicies();
        }

        [Test]
        public async Task NoDataDoesNotProduceAdvisories()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>()
            };
            
            List<AdvisoryMessage> result = await _rule.Evaluate(testData);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task NoAbuseProducesInfo()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>
                {
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 0,
                        Disposition="none"
                    }
                }
            };

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MessageType.info, result[0].MessageType);
            Assert.AreEqual("In the last 30 days, there were 0 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious.", result[0].Text);
        }

        [Test]
        public async Task DispositionNoneWithLessThan50EmailsDeliveredShouldBeInfo()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>
                {
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 29,
                        Disposition="none"
                    },
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 20,
                        Disposition="none"
                    }
                }                
            };
            
            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MessageType.info, result[0].MessageType);
        }

        [Test]
        public async Task DispositionNoneWithMoreThan50EmailsDeliveredShouldBeWarning()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>
                {
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 52,
                        Disposition="none"
                    },
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 10,
                        Disposition="none"
                    }
                }
            };

            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MessageType.warning, result[0].MessageType);
        }

        [Test]
        public async Task DispositionNoneWithMoreThan500EmailsDeliveredShouldBeError()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>
                {
                    new FlaggedTrafficData
                    {
                        Alltraffic = 1000,
                        FlaggedTraffic = 510,
                        Disposition="none"
                    }
                }
            };
            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MessageType.error, result[0].MessageType);
        }

        [Test]
        public async Task DispositionMixedWithLessThan50FlaggedDeliveredShouldBeInfo()
        {
            AbuseEvaluationObject testData = new AbuseEvaluationObject
            {
                Domain = "test.gov.uk",
                FlaggedTrafficData = new List<FlaggedTrafficData>
                {
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 30,
                        Disposition="none"
                    },
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 10,
                        Disposition = "quarantine"
                    },
                    new FlaggedTrafficData
                    {
                        Alltraffic = 100,
                        FlaggedTraffic = 10,
                        Disposition = "reject"
                    }
                }
            };
            List<AdvisoryMessage> result = await _rule.Evaluate(testData);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MessageType.info, result[0].MessageType);
            Assert.AreEqual("In the last 30 days, there were 50 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious. 30 of these were delivered.", result[0].Text);
        }
    }
}