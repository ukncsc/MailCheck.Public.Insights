using System;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Configuration;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers.Configuration;
using NUnit.Framework;

namespace MailCheck.Insights.DataSaver.Test.DataSavers.Configuration
{
    [TestFixture]
    public class ConfigurationDataFactoryTests
    {
        private ConfigurationDataFactory _configurationDataFactory;

        [SetUp]
        public void SetUp()
        {
            _configurationDataFactory = new ConfigurationDataFactory();
        }

        [Test]
        public void CreateMapsPrimitiveProperties()
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 123,
                Pct = 12,
                EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05),
                RecordId = "recordId"
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(123, result.Traffic);
        }

        [TestCase(DmarcResult.pass, 10, 00, 00)]
        [TestCase(DmarcResult.pass, 09, 01, 00)]
        [TestCase(DmarcResult.fail, 00, 10, 00)]
        [TestCase(DmarcResult.fail, 01, 09, 10)]
        public void CreateMapsSpfMisalignedProperties(DmarcResult spfResult, int spfPassCount, int spfFailCount, int expectedMisaligned)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Spf = spfResult,
                SpfPassCount = spfPassCount,
                SpfFailCount = spfFailCount,
                Count = 10
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedMisaligned, result.SpfMisaligned);
        }

        [TestCase(DmarcResult.pass, 10, 00, 00)]
        [TestCase(DmarcResult.pass, 09, 01, 00)]
        [TestCase(DmarcResult.fail, 00, 10, 00)]
        [TestCase(DmarcResult.fail, 01, 09, 10)]
        public void CreateMapsDkimMisalignedProperties(DmarcResult dkimResult, int dkimPassCount, int dkimFailCount, int expectedMisaligned)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Dkim = dkimResult,
                DkimPassCount = dkimPassCount,
                DkimFailCount = dkimFailCount,
                Count = 10
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedMisaligned, result.DkimMisaligned);
        }

        [TestCase(DmarcResult.pass, 0)]
        [TestCase(DmarcResult.fail, 1)]
        [TestCase(null, 1)]
        public void CreateMapsSpfResult(DmarcResult? sourceResult, int expectedFailedCount)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 1,
                Spf = sourceResult
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedFailedCount, result.SpfAuthFail);
        }

        [TestCase(DmarcResult.pass, 0)]
        [TestCase(DmarcResult.fail, 1)]
        [TestCase(null, 1)]
        public void CreateMapsDkimResult(DmarcResult? sourceResult, int expectedFailedCount)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 1,
                Dkim = sourceResult
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedFailedCount, result.DkimAuthFail);
        }

        [TestCase(Policy.none, "none")]
        [TestCase(Policy.quarantine, "quarantine")]
        [TestCase(Policy.reject, "reject")]
        public void CreateMapsPolicy(Policy policy, string expectedResult)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                P = policy
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Policy);
        }

        [TestCase("header.com", null, "header.com")]
        [TestCase(". HeAdEr.CoM. ", null, "header.com")]
        [TestCase(null, "domain.com", "domain.com")]
        [TestCase(null, ". DoMaIn.CoM. ", "domain.com")]
        [TestCase("header.com", "domain.com", "header.com")]
        public void CreateMapsDomain(string headerFrom, string domainFrom, string expectedDomain)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = headerFrom,
                DomainFrom = domainFrom
            };

            ConfigurationData result = _configurationDataFactory.Create(source);

            Assert.AreEqual(expectedDomain, result.Domain);
        }
    }
}
