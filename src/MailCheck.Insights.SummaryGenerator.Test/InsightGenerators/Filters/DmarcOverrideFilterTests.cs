using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Filters
{
    [TestFixture]
    public class DmarcOverrideFilterTests
    {
        private DmarcOverrideFilter _dmarcOverrideFilter;

        [SetUp]
        public void SetUp()
        {
            _dmarcOverrideFilter = new DmarcOverrideFilter();
        }

        [TestCase(0, 0, 0, 0, 0, 0, false)]
        [TestCase(1, 0, 0, 0, 0, 0, true)]
        [TestCase(0, 1, 0, 0, 0, 0, true)]
        [TestCase(0, 0, 1, 0, 0, 0, true)]
        [TestCase(0, 0, 0, 1, 0, 0, true)]
        [TestCase(0, 0, 0, 0, 1, 0, true)]
        [TestCase(0, 0, 0, 0, 0, 1, true)]
        public void FilterShouldOverrideIfDmarcOverrideReasonExists(int arc, int forwarded, int localPolicy, int mailingList, int otherOverrideReason, int trustedForwarder, bool expectedResult)
        {
            RawAbuseData source = new RawAbuseData
            {
                Arc = arc,
                Forwarded = forwarded,
                LocalPolicy = localPolicy,
                MailingList = mailingList,
                OtherOverrideReason = otherOverrideReason,
                TrustedForwarder = trustedForwarder
            };

            bool result = _dmarcOverrideFilter.Filter(source);

            Assert.AreEqual(FilterType.Override, _dmarcOverrideFilter.FilterType);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
