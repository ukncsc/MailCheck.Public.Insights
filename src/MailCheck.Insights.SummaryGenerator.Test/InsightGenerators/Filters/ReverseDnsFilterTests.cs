using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Filters
{
    [TestFixture]
    public class ReverseDnsFilterTests
    {
        private ReverseDnsFilter _reverseDnsFilter;

        [SetUp]
        public void SetUp()
        {
            _reverseDnsFilter = new ReverseDnsFilter();
        }


        [Test]
        public void FilterIgnoresOkTraffic()
        {
            RawAbuseData source = new RawAbuseData();

            bool result = _reverseDnsFilter.Filter(source);

            Assert.AreEqual(true, result);
        }

        [TestCase("unknown")]
        [TestCase("UNKNOWN")]
        [TestCase("UnKnOwN")]
        [TestCase("mismatch")]
        [TestCase("MISMATCH")]
        [TestCase("MiSMaTcH")]
        public void FilterShouldDisallowHostNamesFailingReverseDns(string unknown)
        {
            RawAbuseData source = new RawAbuseData
            {
                HostName = unknown
            };

            bool result = _reverseDnsFilter.Filter(source);

            Assert.AreEqual(FilterType.NonMandatory, _reverseDnsFilter.FilterType);

            Assert.AreEqual(false, result);
        }

        [TestCase("unknown")]
        [TestCase("mismatch")]
        public void FilterShouldAllowKnownProviders(string unknown)
        {
            RawAbuseData source = new RawAbuseData
            {
                HostName = unknown,
                HostProvider = "NHS N3 network"
            };

            bool result = _reverseDnsFilter.Filter(source);

            Assert.AreEqual(FilterType.NonMandatory, _reverseDnsFilter.FilterType);

            Assert.AreEqual(true, result);
        }
    }
}