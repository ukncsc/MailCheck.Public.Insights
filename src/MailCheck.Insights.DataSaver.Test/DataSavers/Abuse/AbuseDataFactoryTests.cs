using System;
using System.Collections.Generic;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers.Abuse;
using MailCheck.Insights.Contracts.Abuse;
using NUnit.Framework;

namespace MailCheck.Insights.DataSaver.Test.DataSavers.Abuse
{
    [TestFixture]
    public class AbuseDataFactoryTests
    {
        private AbuseDataFactory _abuseDataFactory;

        [SetUp]
        public void SetUp()
        {
            _abuseDataFactory = new AbuseDataFactory();
        }

        [TestCaseSource(nameof(ExerciseAbuseDataFactoryTestPermutations))]
        public void CreateMapsPrimitiveProperties(AbuseDataFactoryTestCase testCase)
        {
            AbuseData result = _abuseDataFactory.Create(testCase.EnrichedRecord);

            Assert.AreEqual(99, result.Count);
            Assert.AreEqual("test.gov.uk", result.Domain);
            Assert.AreEqual("uk.gov.test", result.ReverseDomain);
            Assert.AreEqual(new DateTime(2000, 01, 02, 00, 00, 00), result.EffectiveDate);
            Assert.AreEqual(testCase.ExpectedPolicy, result.P);
            Assert.AreEqual(9, result.Pct);
            Assert.AreEqual(testCase.ExpectedFlagged, result.Flagged);
        }

        private static IEnumerable<AbuseDataFactoryTestCase> ExerciseAbuseDataFactoryTestPermutations()
        {
            AbuseDataFactoryTestCase test1 = new AbuseDataFactoryTestCase
            {
                Description = "No Abuse evident should have a flagged count of 0",
                ExpectedFlagged = 0,
                ExpectedPolicy = "none",
                EnrichedRecord = new AggregateReportRecordEnriched("test.gov.uk")
                {
                    HostName = "test.host.com.",
                    EndUserBlockListCount = 0,
                    BounceReflectorBlockListCount = 0,
                    EndUserNetworkBlockListCount = 0,
                    HijackedNetworkBlockListCount = 0,
                    ProxyBlockListCount = 0,
                    SpamSourceBlockListCount = 0,
                    SuspiciousNetworkBlockListCount = 0,
                    MalwareBlockListCount = 0,
                    HeaderFrom = "test.gov.uk",
                    Count = 99,
                    EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05),
                    P = Policy.none,
                    Pct = 9
                }
            };

            AbuseDataFactoryTestCase test2 = new AbuseDataFactoryTestCase
            {
                Description = "Abuse evident with count 99 should have a flagged count of 99",
                ExpectedFlagged = 99,
                ExpectedPolicy = "none",
                EnrichedRecord = new AggregateReportRecordEnriched("test.gov.uk")
                {
                    HostName = "test.host.com.",
                    EndUserBlockListCount = 0,
                    BounceReflectorBlockListCount = 5,
                    EndUserNetworkBlockListCount = 0,
                    HijackedNetworkBlockListCount = 0,
                    ProxyBlockListCount = 0,
                    SpamSourceBlockListCount = 0,
                    SuspiciousNetworkBlockListCount = 0,
                    MalwareBlockListCount = 0,
                    HeaderFrom = "test.gov.uk",
                    Count = 99,
                    EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05),
                    P = Policy.none,
                    Pct = 9
                }
            };

            AbuseDataFactoryTestCase test3 = new AbuseDataFactoryTestCase
            {
                Description = "Invalid policy",
                ExpectedFlagged = 99,
                ExpectedPolicy = "123",
                EnrichedRecord = new AggregateReportRecordEnriched("test.gov.uk")
                {
                    HostName = "test.host.com.",
                    EndUserBlockListCount = 0,
                    BounceReflectorBlockListCount = 5,
                    EndUserNetworkBlockListCount = 0,
                    HijackedNetworkBlockListCount = 0,
                    ProxyBlockListCount = 0,
                    SpamSourceBlockListCount = 0,
                    SuspiciousNetworkBlockListCount = 0,
                    MalwareBlockListCount = 0,
                    HeaderFrom = "test.gov.uk",
                    Count = 99,
                    EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05),
                    P = (Policy)123,
                    Pct = 9
                }
            };

            yield return test1;
            yield return test2;
            yield return test3;
        }
    }
}
