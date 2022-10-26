using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Filters
{
    [TestFixture]
    public class DmarcPassFilterTests
    {
        private DmarcPassFilter _dmarcPassFilter;

        [SetUp]
        public void SetUp()
        {
            _dmarcPassFilter = new DmarcPassFilter();
        }

        [Test]
        public void SpfPass()
        {
            RawAbuseData source = new RawAbuseData
            {
                Spf = "pass"
            };

            bool result = _dmarcPassFilter.Filter(source);

            Assert.AreEqual(FilterType.Mandatory, _dmarcPassFilter.FilterType);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void DkimPass()
        {
            RawAbuseData source = new RawAbuseData
            {
                Dkim = "pass"
            };

            bool result = _dmarcPassFilter.Filter(source);

            Assert.AreEqual(FilterType.Mandatory, _dmarcPassFilter.FilterType);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void NoneSpfOrDkim()
        {
            RawAbuseData source = new RawAbuseData
            {
                Dkim = "none",
                Spf = "none"
            };

            bool result = _dmarcPassFilter.Filter(source);

            Assert.AreEqual(FilterType.Mandatory, _dmarcPassFilter.FilterType);

            Assert.AreEqual(false, result);
        }
    }
}
