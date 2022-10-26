using System.Collections.Generic;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration
{
    [TestFixture]
    public class ExcludedTrafficFilterTests
    {
        private ExcludedTrafficFilter _excludedTrafficFilter;

        [SetUp]
        public void SetUp()
        {
            _excludedTrafficFilter = new ExcludedTrafficFilter();
        }

        [Test]
        public void FilterShouldRemoveExcludedTraffic()
        {
            List<NormalisedRecord> source = new List<NormalisedRecord>()
            {
                new NormalisedRecord {HostProvider = "lsoft.se"},
                new NormalisedRecord {HostProvider = "groups.io"},
                new NormalisedRecord {HostName = "Mismatch"},
                new NormalisedRecord {HostName = "Unknown"},
                new NormalisedRecord {Arc = 1},
                new NormalisedRecord {BounceReflectorBlockListCount = 1},
                new NormalisedRecord {EndUserBlockListCount = 1},
                new NormalisedRecord {EndUserNetworkBlockListCount = 1},
                new NormalisedRecord {HijackedNetworkBlockListCount = 1},
                new NormalisedRecord {MalwareBlockListCount = 1},
                new NormalisedRecord {ProxyBlockListCount = 1},
                new NormalisedRecord {SuspiciousNetworkBlockListCount = 1},
                new NormalisedRecord {SpamSourceBlockListCount = 1},
                new NormalisedRecord
                {
                    Arc = 0,
                    HostName = "testHostName",
                    HostProvider = "testHostProvider",
                    BounceReflectorBlockListCount = 0,
                    EndUserBlockListCount = 0,
                    EndUserNetworkBlockListCount = 0,
                    HijackedNetworkBlockListCount = 0,
                    MalwareBlockListCount = 0,
                    ProxyBlockListCount = 0,
                    SuspiciousNetworkBlockListCount = 0,
                    SpamSourceBlockListCount = 0,
                }
            };

            List<NormalisedRecord> result = _excludedTrafficFilter.Filter(source);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("testHostProvider", result[0].HostProvider);
        }
    }
}