using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Filters
{
    [TestFixture]
    public class BlocklistFilterTests
    {
        private BlocklistFilter _blocklistFilter;

        [SetUp]
        public void SetUp()
        {
            _blocklistFilter = new BlocklistFilter();
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, true)]
        [TestCase(1, 0, 0, 0, 0, 0, 0, 0, false)]
        [TestCase(0, 1, 0, 0, 0, 0, 0, 0, false)]
        [TestCase(0, 0, 1, 0, 0, 0, 0, 0, false)]
        [TestCase(0, 0, 0, 1, 0, 0, 0, 0, false)]
        [TestCase(0, 0, 0, 0, 1, 0, 0, 0, false)]
        [TestCase(0, 0, 0, 0, 0, 1, 0, 0, false)]
        [TestCase(0, 0, 0, 0, 0, 0, 1, 0, false)]
        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, false)]
        public void FilterShouldDisallowBlocklistedTraffic(int bounceReflectorBlocklistCount, int endUserBlocklistCount, int endUserNetworkBlocklistCount, int hijackedNetworkBlocklistCount, int malwareBlocklistCount, int proxyBlocklistCount, int suspiciousNetworkBlocklistCount, int spamSourceBlocklistCount, bool expectedResult)
        {
            RawAbuseData source = new RawAbuseData
            {
                BounceReflectorBlocklistCount = bounceReflectorBlocklistCount,
                EndUserBlocklistCount = endUserBlocklistCount,
                EndUserNetworkBlocklistCount = endUserNetworkBlocklistCount,
                HijackedNetworkBlocklistCount = hijackedNetworkBlocklistCount,
                MalwareBlocklistCount = malwareBlocklistCount,
                ProxyBlocklistCount = proxyBlocklistCount,
                SuspiciousNetworkBlocklistCount = suspiciousNetworkBlocklistCount,
                SpamSourceBlocklistCount = spamSourceBlocklistCount,
            };

            bool result = _blocklistFilter.Filter(source);

            Assert.AreEqual(FilterType.NonMandatory, _blocklistFilter.FilterType);

            Assert.AreEqual(expectedResult, result);
        }
    }
}