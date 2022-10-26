using System;
using System.Collections.Generic;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers.Raw;
using NUnit.Framework;

namespace MailCheck.Insights.DataSaver.Test.DataSavers.Raw
{
    [TestFixture]
    public class NormalisedRecordFactoryTests
    {
        private NormalisedRecordFactory _normalisedRecordFactory;

        [SetUp]
        public void SetUp()
        {
            _normalisedRecordFactory = new NormalisedRecordFactory();
        }

        [Test]
        public void CreateMapsPrimitiveProperties()
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual("DkimAuthResult1,DkimAuthResult2", result.DkimAuthResults);
            Assert.AreEqual("DomainFrom", result.DomainFrom);
            Assert.AreEqual("EnvelopeFrom", result.EnvelopeFrom);
            Assert.AreEqual("EnvelopeTo", result.EnvelopeTo);
            Assert.AreEqual("Fo", result.Fo);
            Assert.AreEqual("HeaderFrom", result.HeaderFrom);
            Assert.AreEqual("HostAsDescription", result.HostAsDescription);
            Assert.AreEqual("HostCountry", result.HostCountry);
            Assert.AreEqual("HostName", result.HostName);
            Assert.AreEqual("HostOrgDomain", result.HostOrgDomain);
            Assert.AreEqual("HostProvider", result.HostProvider);
            Assert.AreEqual("HostSourceIp", result.HostSourceIp);
            Assert.AreEqual("OrganisationDomainFrom", result.OrganisationDomainFrom);
            Assert.AreEqual("ReporterOrgName", result.ReporterOrgName);
            Assert.AreEqual("ReportId", result.ReportId);
            Assert.AreEqual("SpfAuthResult1,SpfAuthResult2", result.SpfAuthResults);

            Assert.AreEqual(1, result.BounceReflectorBlockListCount);
            Assert.AreEqual(2, result.DkimFailCount);
            Assert.AreEqual(3, result.DkimPassCount);
            Assert.AreEqual(4, result.EndUserBlockListCount);
            Assert.AreEqual(5, result.EndUserNetworkBlockListCount);
            Assert.AreEqual(6, result.HijackedNetworkBlockListCount);
            Assert.AreEqual(7, result.HostAsNumber);
            Assert.AreEqual(8, result.MalwareBlockListCount);
            Assert.AreEqual(9, result.Pct);
            Assert.AreEqual(10, result.ProxyBlockListCount);
            Assert.AreEqual(11, result.RecordId);
            Assert.AreEqual(12, result.SpamSourceBlockListCount);
            Assert.AreEqual(13, result.SpfFailCount);
            Assert.AreEqual(14, result.SpfPassCount);
            Assert.AreEqual(15, result.SuspiciousNetworkBlockListCount);
        }

        [Test]
        public void CreateMapsDate()
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.EffectiveDate = new DateTime(2000, 01, 02, 03, 04, 05);

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(new DateTime(2000, 01, 02), result.EffectiveDate);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsArc(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Arc = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.Arc);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsForwarded(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Forwarded = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.Forwarded);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsLocalPolicy(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.LocalPolicy = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.LocalPolicy);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsMailingList(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.MailingList = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.MailingList);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsOtherOverrideReason(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.OtherOverrideReason = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.OtherOverrideReason);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsSampledOut(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.SampledOut = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedCount, result.SampledOut);
        }

        [TestCase(true, 99)]
        [TestCase(false, 0)]
        public void CreateMapsTrustedForwarder(bool truthiness, int expectedCount)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.TrustedForwarder = truthiness;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);
            Assert.AreEqual(expectedCount, result.TrustedForwarder);
        }

        [TestCase(Policy.none, "none")]
        [TestCase(Policy.quarantine, "quarantine")]
        [TestCase(Policy.reject, "reject")]
        [TestCase(null, null)]
        public void CreateMapsDisposition(Policy? sourcePolicy, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Disposition = sourcePolicy;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Disposition);
        }

        [TestCase(Policy.none, "none")]
        [TestCase(Policy.quarantine, "quarantine")]
        [TestCase(Policy.reject, "reject")]
        public void CreateMapsP(Policy sourcePolicy, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.P = sourcePolicy;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.P);
        }

        [TestCase(DmarcResult.fail, "fail")]
        [TestCase(DmarcResult.pass, "pass")]
        [TestCase(null, null)]
        public void CreateMapsDkim(DmarcResult? sourceDmarcResult, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Dkim = sourceDmarcResult;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Dkim);
        }

        [TestCase(DmarcResult.fail, "fail")]
        [TestCase(DmarcResult.pass, "pass")]
        [TestCase(null, null)]
        public void CreateMapsSpf(DmarcResult? sourceDmarcResult, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Spf = sourceDmarcResult;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Spf);
        }

        [TestCase(Alignment.r, "r")]
        [TestCase(Alignment.s, "s")]
        [TestCase(null, null)]
        public void CreateMapsAdkim(Alignment? sourceAlignment, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Adkim = sourceAlignment;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Adkim);
        }

        [TestCase(Alignment.r, "r")]
        [TestCase(Alignment.s, "s")]
        [TestCase(null, null)]
        public void CreateMapsAspf(Alignment? sourceAlignment, string expectedResult)
        {
            AggregateReportRecordEnriched source = CreateDefaultSourceRecord();
            source.Aspf = sourceAlignment;

            NormalisedRecord result = _normalisedRecordFactory.Create(source);

            Assert.AreEqual(expectedResult, result.Aspf);
        }

        private AggregateReportRecordEnriched CreateDefaultSourceRecord()
        {
            AggregateReportRecordEnriched source = new AggregateReportRecordEnriched("Id")
            {
                Adkim = Alignment.r,
                Aspf = Alignment.r,
                Arc = false,
                BounceReflectorBlockListCount = 1,
                Count = 99,
                Disposition = Policy.none,
                Dkim = DmarcResult.fail,
                DkimAuthResults = new List<string> { "DkimAuthResult1", "DkimAuthResult2" },
                DkimFailCount = 2,
                DkimPassCount = 3,
                DomainFrom = "DomainFrom",
                EffectiveDate = DateTime.UnixEpoch,
                EndUserBlockListCount = 4,
                EndUserNetworkBlockListCount = 5,
                EnvelopeFrom = "EnvelopeFrom",
                EnvelopeTo = "EnvelopeTo",
                Fo = "Fo",
                Forwarded = false,
                HeaderFrom = "HeaderFrom",
                HijackedNetworkBlockListCount = 6,
                HostAsDescription = "HostAsDescription",
                HostAsNumber = 7,
                HostCountry = "HostCountry",
                HostName = "HostName",
                HostOrgDomain = "HostOrgDomain",
                HostProvider = "HostProvider",
                HostSourceIp = "HostSourceIp",
                LocalPolicy = false,
                MailingList = false,
                MalwareBlockListCount = 8,
                OrganisationDomainFrom = "OrganisationDomainFrom",
                OtherOverrideReason = false,
                P = Policy.none,
                Pct = 9,
                ProxyBlockListCount = 10,
                RecordId = "11",
                ReporterOrgName = "ReporterOrgName",
                ReportId = "ReportId",
                SampledOut = false,
                Sp = Policy.none,
                SpamSourceBlockListCount = 12,
                Spf = DmarcResult.fail,
                SpfAuthResults = new List<string> { "SpfAuthResult1", "SpfAuthResult2" },
                SpfFailCount = 13,
                SpfPassCount = 14,
                SuspiciousNetworkBlockListCount = 15,
                TrustedForwarder = false,
            };

            return source;
        }
    }
}
