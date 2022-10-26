using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Factories;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Abuse
{
    [TestFixture]
    public class AbuseInsightGeneratorTests
    {
        private AbuseInsightGenerator _abuseInsightGenerator;
        private IAbuseInsightGeneratorDao _dao;
        private ILogger<AbuseInsightGenerator> _log;
        private ISummaryGeneratorConfig _config;
        private IEvaluator<AbuseEvaluationObject> _evaluator;
        private IFlaggedTrafficDataFactory _flaggedTrafficDataFactory;

        private static Guid AbuseGuid = new Guid("917c7b26-ec74-4474-bd31-241f6b3b752f");
        private static Guid NoDataGuid = new Guid("2c33cb59-5b6f-48e7-ab2c-be151270c401");

        [SetUp]
        public void Setup()
        {
            _dao = A.Fake<IAbuseInsightGeneratorDao>();
            _log = A.Fake<ILogger<AbuseInsightGenerator>>();
            _config = A.Fake<ISummaryGeneratorConfig>();
            _flaggedTrafficDataFactory = A.Fake<IFlaggedTrafficDataFactory>();

            IRule<AbuseEvaluationObject> policyAll = new AbuseRateWithAllPolicies();

            List<IRule<AbuseEvaluationObject>> rules = new List<IRule<AbuseEvaluationObject>> { policyAll };
            _evaluator = new Evaluator<AbuseEvaluationObject>(rules);

            _abuseInsightGenerator = new AbuseInsightGenerator(_dao, _log, _evaluator, _config, _flaggedTrafficDataFactory);
        }

        [TestCaseSource(nameof(ExerciseGenerateAbuseInsightsTestPermutations))]
        public async Task ExerciseGenerateInsights(AbuseInsightGeneratorTestCase testCase)
        {
            List<RawAbuseData> rawAbuseData = new List<RawAbuseData>();
            A.CallTo(() => _dao.GetRawData("test.gov.uk", DateTime.MinValue, DateTime.MaxValue)).Returns(rawAbuseData);
            A.CallTo(() => _flaggedTrafficDataFactory.Create(rawAbuseData)).Returns(testCase.Data);

            List<RawSubdomainAbuseData> rawSubdomainAbuseData = new List<RawSubdomainAbuseData>();
            A.CallTo(() => _dao.GetRawSubdomainData("test.gov.uk", DateTime.MinValue, DateTime.MaxValue)).Returns(rawSubdomainAbuseData);
            A.CallTo(() => _flaggedTrafficDataFactory.Create(rawSubdomainAbuseData)).Returns(testCase.SubdomainData);

            Insight result = await _abuseInsightGenerator.GenerateInsights("test.gov.uk", DateTime.MinValue, DateTime.MaxValue);

            Assert.AreEqual(testCase.ExpectedAdvisories.Count, result.Advisories.Count);
            if(testCase.ExpectedAdvisories.Count > 0)
            {
                Assert.AreEqual(testCase.ExpectedAdvisories[0].MessageType, result.Advisories[0].MessageType);
                Assert.AreEqual(testCase.ExpectedAdvisories[0].Text, result.Advisories[0].Text);
            }
        }

        private static IEnumerable<AbuseInsightGeneratorTestCase> ExerciseGenerateAbuseInsightsTestPermutations()
        {
            FlaggedTrafficData DispositionNoneLessThan50Flagged = new FlaggedTrafficData
            {
                FlaggedTraffic = 10,
                Alltraffic = 1000,
                Disposition = "none"
            };

            FlaggedTrafficData DispositionNoneLessThan500Flagged = new FlaggedTrafficData
            {
                FlaggedTraffic = 100,
                Alltraffic = 1000,
                Disposition = "none"
            };

            FlaggedTrafficData DispositionNoneMoreThan250Flagged = new FlaggedTrafficData
            {
                FlaggedTraffic = 275,
                Alltraffic = 1000,
                Disposition = "none"
            };

            FlaggedTrafficData DispositionQuarantineFlagged = new FlaggedTrafficData
            {
                FlaggedTraffic = 25,
                Alltraffic = 1000,
                Disposition = "quarantine"
            };

            FlaggedTrafficData DispositionRejectFlagged = new FlaggedTrafficData
            {
                FlaggedTraffic = 25,
                Alltraffic = 1000,
                Disposition = "reject"
            };

            FlaggedTrafficData NoFlaggedTraffic = new FlaggedTrafficData
            {
                FlaggedTraffic = 0,
                Alltraffic = 100,
                Disposition = "none"
            };

            AbuseInsightGeneratorTestCase test1 = new AbuseInsightGeneratorTestCase()
            {
                Description = "If flagged traffic delivered < 50 then raise informational advisory",
                Data = new List<FlaggedTrafficData> { DispositionNoneLessThan50Flagged, DispositionNoneLessThan50Flagged },
                SubdomainData = new List<FlaggedTrafficData> { DispositionNoneLessThan50Flagged, DispositionNoneLessThan50Flagged },
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(AbuseGuid, MessageType.info, "In the last 30 days, there were 40 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious. 40 of these were delivered.", "") }
            };

            AbuseInsightGeneratorTestCase test2 = new AbuseInsightGeneratorTestCase()
            {
                Description = "If flagged traffic delivered >= 50 and <= 500 then raise warning advisory",
                Data = new List<FlaggedTrafficData> { DispositionNoneLessThan500Flagged },
                SubdomainData = new List<FlaggedTrafficData> { DispositionNoneLessThan500Flagged },
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(AbuseGuid, MessageType.warning, "In the last 30 days, there were 200 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious. 200 of these were delivered.", "") }
            };

            AbuseInsightGeneratorTestCase test3 = new AbuseInsightGeneratorTestCase()
            {
                Description = "If flagged traffic delivered > 500 then raise error advisory",
                Data = new List<FlaggedTrafficData> { DispositionNoneMoreThan250Flagged, DispositionNoneMoreThan250Flagged },
                SubdomainData = new List<FlaggedTrafficData> { DispositionNoneMoreThan250Flagged, DispositionNoneMoreThan250Flagged },
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(AbuseGuid, MessageType.error, "In the last 30 days, there were 1100 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious. 1100 of these were delivered.", "") }
            };

            AbuseInsightGeneratorTestCase test4 = new AbuseInsightGeneratorTestCase()
            {
                Description = "If flagged traffic delivered < 500 with mixed dispositions then raise warning advisory",
                Data = new List<FlaggedTrafficData> { DispositionNoneLessThan500Flagged, DispositionQuarantineFlagged, DispositionRejectFlagged },
                SubdomainData = new List<FlaggedTrafficData> { DispositionNoneLessThan500Flagged, DispositionQuarantineFlagged, DispositionRejectFlagged },
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(AbuseGuid, MessageType.warning, "In the last 30 days, there were 300 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious. 200 of these were delivered.", "") }
            };

            AbuseInsightGeneratorTestCase test5 = new AbuseInsightGeneratorTestCase()
            {
                Description = "If no flagged traffic, raise informational without number of emails delivered",
                Data = new List<FlaggedTrafficData> { NoFlaggedTraffic },
                SubdomainData = new List<FlaggedTrafficData> { NoFlaggedTraffic },
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(AbuseGuid, MessageType.info, "In the last 30 days, there were 0 emails using test.gov.uk (or sub-domains) that were flagged as potentially malicious.", "") }
            };

            AbuseInsightGeneratorTestCase test6 = new AbuseInsightGeneratorTestCase()
            {
                Description = "No traffic should raise an informational",
                Data = new List<FlaggedTrafficData>(),
                SubdomainData = new List<FlaggedTrafficData>(),
                ExpectedAdvisories = new List<AdvisoryMessage> { new AdvisoryMessage(NoDataGuid, MessageType.info, "We have insufficient DMARC reporting data over the last 30 days to produce abuse insights for test.gov.uk", "")}
            };

            yield return test1;
            yield return test2;
            yield return test3;
            yield return test4;
            yield return test5;
            yield return test6;

        }
    }
}