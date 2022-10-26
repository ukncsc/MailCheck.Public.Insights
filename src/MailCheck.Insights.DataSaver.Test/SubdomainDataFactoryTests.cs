using System;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers.Subdomains;
using NUnit.Framework;

namespace MailCheck.Insights.DataSaver.Test
{
    [TestFixture]
    public class SubdomainDataFactoryTests
    {
        private SubdomainDataFactory _subdomainDataFactory;

        [SetUp]
        public void SetUp()
        {
            _subdomainDataFactory = new SubdomainDataFactory();
        }

        [Test]
        public void CreateMapsPrimitiveProperties()
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05),
                RecordId = "recordId",
                Count = 123
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(new DateTime(2000, 01, 02), result.EffectiveDate);
            Assert.AreEqual("recordId", result.RecordId);
            Assert.AreEqual(123, result.AllTrafficCount);
        }

        [TestCase(Policy.none, DmarcResult.pass, DmarcResult.pass, 0, 0, 0, 0)]
        [TestCase(Policy.none, DmarcResult.pass, DmarcResult.fail, 1, 0, 0, 0)]
        [TestCase(Policy.none, DmarcResult.pass, null, 1, 0, 0, 0)]
        [TestCase(Policy.none, DmarcResult.fail, DmarcResult.pass, 1, 0, 0, 0)]
        [TestCase(Policy.none, DmarcResult.fail, DmarcResult.fail, 1, 1, 0, 0)]
        [TestCase(Policy.none, DmarcResult.fail, null, 1, 1, 0, 0)]
        [TestCase(Policy.none, null, DmarcResult.pass, 1, 0, 0, 0)]
        [TestCase(Policy.none, null, DmarcResult.fail, 1, 1, 0, 0)]
        [TestCase(Policy.none, null, null, 1, 1, 0, 0)]

        [TestCase(null, DmarcResult.pass, DmarcResult.pass, 0, 0, 0, 0)]
        [TestCase(null, DmarcResult.pass, DmarcResult.fail, 1, 0, 0, 0)]
        [TestCase(null, DmarcResult.pass, null, 1, 0, 0, 0)]
        [TestCase(null, DmarcResult.fail, DmarcResult.pass, 1, 0, 0, 0)]
        [TestCase(null, DmarcResult.fail, DmarcResult.fail, 1, 1, 0, 0)]
        [TestCase(null, DmarcResult.fail, null, 1, 1, 0, 0)]
        [TestCase(null, null, DmarcResult.pass, 1, 0, 0, 0)]
        [TestCase(null, null, DmarcResult.fail, 1, 1, 0, 0)]
        [TestCase(null, null, null, 1, 1, 0, 0)]

        [TestCase(Policy.quarantine, DmarcResult.pass, DmarcResult.pass, 0, 0, 0, 0)]
        [TestCase(Policy.quarantine, DmarcResult.pass, DmarcResult.fail, 0, 0, 1, 0)]
        [TestCase(Policy.quarantine, DmarcResult.pass, null, 0, 0, 1, 0)]
        [TestCase(Policy.quarantine, DmarcResult.fail, DmarcResult.pass, 0, 0, 1, 0)]
        [TestCase(Policy.quarantine, DmarcResult.fail, DmarcResult.fail, 0, 0, 1, 1)]
        [TestCase(Policy.quarantine, DmarcResult.fail, null, 0, 0, 1, 1)]
        [TestCase(Policy.quarantine, null, DmarcResult.pass, 0, 0, 1, 0)]
        [TestCase(Policy.quarantine, null, DmarcResult.fail, 0, 0, 1, 1)]
        [TestCase(Policy.quarantine, null, null, 0, 0, 1, 1)]

        [TestCase(Policy.reject, DmarcResult.pass, DmarcResult.pass, 0, 0, 0, 0)]
        [TestCase(Policy.reject, DmarcResult.pass, DmarcResult.fail, 0, 0, 1, 0)]
        [TestCase(Policy.reject, DmarcResult.pass, null, 0, 0, 1, 0)]
        [TestCase(Policy.reject, DmarcResult.fail, DmarcResult.pass, 0, 0, 1, 0)]
        [TestCase(Policy.reject, DmarcResult.fail, DmarcResult.fail, 0, 0, 1, 1)]
        [TestCase(Policy.reject, DmarcResult.fail, null, 0, 0, 1, 1)]
        [TestCase(Policy.reject, null, DmarcResult.pass, 0, 0, 1, 0)]
        [TestCase(Policy.reject, null, DmarcResult.fail, 0, 0, 1, 1)]
        [TestCase(Policy.reject, null, null, 0, 0, 1, 1)]
        public void CreateMapsSpfAndDkimResults(Policy? disposition, DmarcResult? spfResult, DmarcResult? dkimResult, int dkimOrSpfFailedNoneCount, int dkimAndSpfFailedNoneCount, int dkimOrSpfFailedQuarantineOrRejectCount, int dkimAndSpfFailedQuarantineOrRejectCount)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 1,
                Spf = spfResult,
                Dkim = dkimResult,
                Disposition = disposition
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(dkimOrSpfFailedNoneCount, result.DkimOrSpfFailedNoneCount);
            Assert.AreEqual(dkimAndSpfFailedNoneCount, result.DkimAndSpfFailedNoneCount);
            Assert.AreEqual(dkimOrSpfFailedQuarantineOrRejectCount, result.DkimOrSpfFailedQuarantineOrRejectCount);
            Assert.AreEqual(dkimAndSpfFailedQuarantineOrRejectCount, result.DkimAndSpfFailedQuarantineOrRejectCount);
        }

        [TestCase("header.com", null, "header.com", "com.header")]
        [TestCase(". HeAdEr.CoM. ", null, "header.com", "com.header")]
        [TestCase(null, "domain.com", "domain.com", "com.domain")]
        [TestCase(null, ". DoMaIn.CoM. ", "domain.com", "com.domain")]
        [TestCase("header.com", "domain.com", "header.com", "com.header")]
        public void CreateMapsDomain(string headerFrom, string domainFrom, string expectedDomain,
            string expectedReverseDomain)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = headerFrom,
                DomainFrom = domainFrom
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(expectedDomain, result.Domain);
            Assert.AreEqual(expectedReverseDomain, result.ReverseDomain);
        }

        [TestCase("Unknown", 1)]
        [TestCase("Mismatch", 1)]
        [TestCase("SomethingElse", 0)]
        public void CreateFlagsUnknownOrMismatchHost(string hostName, int expectedFlaggedCount)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 1,
                HostName = hostName
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(expectedFlaggedCount, result.BlockListOrFailedReverseDnsCount);
        }

        [Test]
        public void CreateDoesNotFlagIfNotBlocklisted()
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 123,
                HostName = "SomethingOk",
                EndUserBlockListCount = 0,
                BounceReflectorBlockListCount = 0,
                EndUserNetworkBlockListCount = 0,
                HijackedNetworkBlockListCount = 0,
                ProxyBlockListCount = 0,
                SpamSourceBlockListCount = 0,
                SuspiciousNetworkBlockListCount = 0,
                MalwareBlockListCount = 0,
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(0, result.BlockListOrFailedReverseDnsCount);
        }

        [TestCase(1, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 1, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 1, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 1, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 0, 1, 0, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 1, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 1, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 1)]
        public void CreateFlagsIfBlockListed(int endUserBlockListCount, int bounceReflectorBlockListCount, int endUserNetworkBlockListCount, int hijackedNetworkBlockListCount, int proxyBlockListCount, int spamSourceBlockListCount, int suspiciousNetworkBlockListCount, int malwareBlockListCount)
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                HeaderFrom = "test.com",
                Count = 123,
                EndUserBlockListCount = endUserBlockListCount,
                BounceReflectorBlockListCount = bounceReflectorBlockListCount,
                EndUserNetworkBlockListCount = endUserNetworkBlockListCount,
                HijackedNetworkBlockListCount = hijackedNetworkBlockListCount,
                ProxyBlockListCount = proxyBlockListCount,
                SpamSourceBlockListCount = spamSourceBlockListCount,
                SuspiciousNetworkBlockListCount = suspiciousNetworkBlockListCount,
                MalwareBlockListCount = malwareBlockListCount
            };

            SubdomainData result = _subdomainDataFactory.Create(source);

            Assert.AreEqual(123, result.BlockListOrFailedReverseDnsCount);
        }
    }
}
