using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.Test.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Subdomains
{
    public class SubdomainsInsightGeneratorTests
    {
        private SubdomainInsightGenerator _subdomainInsightGenerator;
        private ISubdomainInsightGeneratorDao _dao;
        private ISubdomainInsightAdvisoryMessageGenerator _advisoryMessageGenerator;
        private ILogger<SubdomainInsightGenerator> _log;
        private IEvaluator<SubdomainsEvaluationObject> _evaluator;
        private static Guid Id = new Guid("3175d33b-735c-4671-84b2-9bb2a2d98983");
        private ISummaryGeneratorConfig _config;

        [SetUp]
        public void Setup()
        {
            _dao = A.Fake<ISubdomainInsightGeneratorDao>();
            _log = A.Fake<ILogger<SubdomainInsightGenerator>>();
            _config = A.Fake<ISummaryGeneratorConfig>();
            _advisoryMessageGenerator = new SubdomainInsightAdvisoryMessageGenerator(_config);
            IRule<SubdomainsEvaluationObject> rule1 = new DispositionNone(_advisoryMessageGenerator);
            IRule<SubdomainsEvaluationObject> rule2 = new DispositionQuarantineOrRejectLessThan5Percent(_advisoryMessageGenerator);
            IRule<SubdomainsEvaluationObject> rule3 = new DispositionQuarantineOrRejectMoreThan5Percent(_advisoryMessageGenerator);


            List<IRule<SubdomainsEvaluationObject>> rules = new List<IRule<SubdomainsEvaluationObject>> { rule1, rule2, rule3 };
            _evaluator = new Evaluator<SubdomainsEvaluationObject>(rules);

            _subdomainInsightGenerator = new SubdomainInsightGenerator(_dao, _log, _evaluator);
        }

        [Test]
        public async Task NoSubdomainsMessage()
        {
            string id = "test.gov.uk";
            Insight result = await _subdomainInsightGenerator.GenerateInsights(id, DateTime.MinValue, DateTime.MaxValue);
            Assert.AreEqual(1, result.Advisories.Count);
            Assert.AreEqual($"Mail Check did not detect any subdomains of {id} (Note we only show subdomains with > 10 emails in the last 30 days)", result.Advisories[0].Text);
            Assert.AreEqual(MessageType.info, result.Advisories[0].MessageType);
        }

        [Test]
        public async Task Limit150Subdomains()
        {
            string id = "test.gov.uk";

            List<Contracts.Raw.NormalisedRecord> list = new List<Contracts.Raw.NormalisedRecord>();

            for (int i = 0; i < 151; i++)
            {
                list.Add(new Contracts.Raw.NormalisedRecord
                {
                    P = "none",
                    Count = 15,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = $"subdomain{i}.test.gov.uk"
                });
            }

            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", string.Empty, string.Empty, list);

            A.CallTo(() => _dao.GetRawData("test.gov.uk", A<DateTime>._, A<DateTime>._)).Returns(rawData);
            Insight result = await _subdomainInsightGenerator.GenerateInsights(id, DateTime.MinValue, DateTime.MaxValue);
            Assert.AreEqual(1, result.Advisories.Count);
            Assert.IsTrue(result.Advisories[0].MarkDown.Contains("and a further 1 subdomains"));
        }

        [TestCaseSource(nameof(ExerciseGenerateConfigInsightsTestPermutations))]
        public async Task ExerciseGenerateInsights(SubdomainsInsightGeneratorTestCase testCase)
        {
            A.CallTo(() => _dao.GetRawData("test.gov.uk", A<DateTime>._, A<DateTime>._)).Returns(testCase.Data);

            Insight result = await _subdomainInsightGenerator.GenerateInsights("test.gov.uk", DateTime.MinValue, DateTime.MaxValue);

            Assert.AreEqual(testCase.ExpectedAdvisories.Count, result.Advisories.Count);

            for(int i = 0; i < testCase.ExpectedAdvisories.Count; i++)
            {
                Assert.AreEqual(testCase.ExpectedAdvisories[i].Text, result.Advisories[i].Text);
                Assert.AreEqual(testCase.ExpectedAdvisories[i].MessageType, result.Advisories[i].MessageType);
            }
        }

        private static IEnumerable<SubdomainsInsightGeneratorTestCase> ExerciseGenerateConfigInsightsTestPermutations()
        {
            yield return GetSubdomainsWithMultipleAdvisory();
            yield return GetSubdomainsWithPolicyOfNoneAndParentRejectPolicy();
            yield return GetSubdomainsWithPolicyOfQuarOrRejectAndParentQuarantinePolicy();
            yield return GetSubdomainsWithNoPolicyAndParentQuarantinePolicy();
            yield return GetSubdomainsWithNoPolicyAndParentNoneSPPolicy();
            yield return AllSubdomainsLessThan10EmailsProducesNoSubdomainsMessage();
        }

        private static SubdomainsInsightGeneratorTestCase GetSubdomainsWithMultipleAdvisory()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", string.Empty, string.Empty, new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    P = "none",
                    Count = 15,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "one.test.gov.uk"
                },
                new Contracts.Raw.NormalisedRecord
                {
                    P = "reject",
                    Count = 15,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "two.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "Subdomains with multiple advisories raised",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.warning, "In the last 30 days, Mail Check saw 1 subdomains of test.gov.uk that had a DMARC policy of 'none'. Some or all of these have SPF and DKIM configuration issues.", ""),
                    new AdvisoryMessage(Id, MessageType.warning, "In the last 30 days, Mail Check saw 1 subdomains of test.gov.uk that were well configured with a strong DMARC policy of quarantine or reject, but with possible legitimate systems that need configuring with SPF and DKIM.", "")}
            };
        }

        private static SubdomainsInsightGeneratorTestCase GetSubdomainsWithPolicyOfNoneAndParentRejectPolicy()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", "reject", string.Empty, new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    P = "none",
                    Count = 51,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "one.test.gov.uk"
                },
                new Contracts.Raw.NormalisedRecord
                {
                    P = "none",
                    Count = 15,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "two.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "Subdomain with policy of none with parent having reject policy",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.warning, "In the last 30 days, Mail Check saw 2 subdomains of test.gov.uk that had a DMARC policy of 'none'. Some or all of these have SPF and DKIM configuration issues.", "")}
            };
        }

        private static SubdomainsInsightGeneratorTestCase GetSubdomainsWithPolicyOfQuarOrRejectAndParentQuarantinePolicy()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", "quarantine", string.Empty, new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    P = "reject",
                    Count = 15,
                    Spf = "pass",
                    Dkim = "pass",
                    Domain = "one.test.gov.uk"
                },
                new Contracts.Raw.NormalisedRecord
                {
                    P = "quarantine",
                    Count = 35,
                    Spf = "pass",
                    Dkim = "pass",
                    Domain = "two.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "Subdomain with policy of quarantine or reject with parent having quarantine policy",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.success, "In the last 30 days, Mail Check saw 2 subdomains of test.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.", "")}
            };
        }

        private static SubdomainsInsightGeneratorTestCase GetSubdomainsWithNoPolicyAndParentQuarantinePolicy()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", "quarantine", string.Empty, new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    Count = 15,
                    Spf = "pass",
                    Dkim = "pass",
                    Domain = "one.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "Subdomain with no policy with parent having quarantine policy",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.success, "In the last 30 days, Mail Check saw 1 subdomains of test.gov.uk that were well configured with a strong DMARC policy of quarantine or reject.", "")}
            };
        }

        private static SubdomainsInsightGeneratorTestCase GetSubdomainsWithNoPolicyAndParentNoneSPPolicy()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", "quarantine", "none", new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    Count = 15,
                    Spf = "pass",
                    Dkim = "pass",
                    Domain = "one.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "Subdomain with no policy with parent having none subpolicy",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.warning, "In the last 30 days, Mail Check saw 1 subdomains of test.gov.uk that had a DMARC policy of 'none'. Some or all of these have SPF and DKIM configuration issues.", "")}
            };
        }

        private static SubdomainsInsightGeneratorTestCase AllSubdomainsLessThan10EmailsProducesNoSubdomainsMessage()
        {
            SubdomainsRawData rawData = new SubdomainsRawData("test.gov.uk", string.Empty, string.Empty, new List<Contracts.Raw.NormalisedRecord>
            {
                new Contracts.Raw.NormalisedRecord
                {
                    P = "none",
                    Count = 9,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "one.test.gov.uk"
                },
                new Contracts.Raw.NormalisedRecord
                {
                    P = "reject",
                    Count = 4,
                    Spf = "pass",
                    Dkim = "pass",
                    Domain = "two.test.gov.uk"
                },
                new Contracts.Raw.NormalisedRecord
                {
                    P = "quarantine",
                    Count = 6,
                    Spf = "fail",
                    Dkim = "fail",
                    Domain = "three.test.gov.uk"
                }
            });

            return new SubdomainsInsightGeneratorTestCase()
            {
                Description = "All subdomains have < 10 emails should only produce no subdomains message",
                Data = rawData,
                ExpectedAdvisories = new List<AdvisoryMessage>
                    {new AdvisoryMessage(Id, MessageType.info, "Mail Check did not detect any subdomains of test.gov.uk (Note we only show subdomains with > 10 emails in the last 30 days)", ""),}
            };
        }
    }
}