using System;
using System.Collections.Generic;
using FakeItEasy;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration
{
    [TestFixture]
    public class ConfigurationEvaluationObjectFactoryTests
    {
        private ConfigurationEvaluationObjectFactory _configurationEvaluationObjectFactory;
        private ISummaryGeneratorConfig _config;
        private IProviderAliasProvider _providerAliasProvider;

        [SetUp]
        public void SetUp()
        {
            _config = A.Fake<ISummaryGeneratorConfig>();
            _providerAliasProvider = A.Fake<IProviderAliasProvider>();
            _configurationEvaluationObjectFactory = new ConfigurationEvaluationObjectFactory(_config, _providerAliasProvider);
        }

        [Test]
        public void CreateMapsRootProperties()
        {
            _config.MailCheckUrlPrefix = "testUrlPrefix";

            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider", P = "reject", Count = 20, Dkim = "pass", DkimPassCount = 20, Spf = "pass", SpfPassCount = 20 },
                new NormalisedRecord { HostProvider = "testProvider", P = "quarantine", Count = 40, Dkim = "pass", DkimPassCount = 40, Spf = "fail", SpfPassCount = 40 },
                new NormalisedRecord { HostProvider = "testProvider", P = "none", Count = 60, Dkim = "fail", DkimPassCount = 60, Spf = "pass", SpfPassCount = 60 },
                new NormalisedRecord { HostProvider = "testProvider", P = "none", Count = 80, Dkim = "fail", DkimPassCount = 80, Spf = "fail", SpfPassCount = 80 },
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001,01,01), new DateTime(2002,02,02));

            Assert.AreEqual("testDomain", result.Domain);
            Assert.AreEqual(new DateTime(2001, 01, 01), result.StartDate);
            Assert.AreEqual(new DateTime(2002, 02, 02), result.EndDate);
            Assert.AreEqual("testUrlPrefix", result.Url);
            Assert.AreEqual(200, result.TotalTraffic);
            Assert.AreEqual(40, result.PercentFailingDmarc);
            Assert.AreEqual(70, result.PercentNone);
            Assert.AreEqual(30, result.PercentQuarantineOrReject);
            Assert.AreEqual("none", result.LatestPolicy);
        }

        [Test]
        public void CreateGroupsProviders()
        {
            A.CallTo(() => _providerAliasProvider.GetProviderAlias(A<string>._, A<string>._)).ReturnsLazily((string x, string y) => x);

            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider1", Count = 17 },
                new NormalisedRecord { HostProvider = "testProvider2", Count = 18 },
                new NormalisedRecord { HostProvider = "testProvider2", Count = 19 },
                new NormalisedRecord { HostProvider = "testProvider3", Count = 21 },
                new NormalisedRecord { HostProvider = "testProvider3", Count = 22 },
                new NormalisedRecord { HostProvider = "testProvider3", Count = 23 },
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(120, result.TotalTraffic);
            Assert.AreEqual(3, result.ProviderCalculations.Count);

            Assert.AreEqual(17, result.ProviderCalculations[0].TotalMail);
            Assert.AreEqual(37, result.ProviderCalculations[1].TotalMail);
            Assert.AreEqual(66, result.ProviderCalculations[2].TotalMail);

            Assert.AreEqual("testProvider1", result.ProviderCalculations[0].Name);
            Assert.AreEqual("testProvider2", result.ProviderCalculations[1].Name);
            Assert.AreEqual("testProvider3", result.ProviderCalculations[2].Name);
        }

        [Test]
        public void CreateMapsProviderPolicy()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider1", P = "reject", Count = 38 },
                new NormalisedRecord { HostProvider = "testProvider1", P = "quarantine", Count = 60},
                new NormalisedRecord { HostProvider = "testProvider1", P = "none", Count = 102 },
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);

            Assert.AreEqual(49, result.ProviderCalculations[0].PercentQuarantineOrReject);
            Assert.AreEqual(51, result.ProviderCalculations[0].PercentNone);
            Assert.AreEqual(200, result.ProviderCalculations[0].TotalMail);
        }

        [Test]
        public void CreateMapsProviderDmarcFails()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider", Count = 46, Dkim = "pass", Spf = "pass" },
                new NormalisedRecord { HostProvider = "testProvider", Count = 48, Dkim = "pass", Spf = "fail" },
                new NormalisedRecord { HostProvider = "testProvider", Count = 52, Dkim = "fail", Spf = "pass" },
                new NormalisedRecord { HostProvider = "testProvider", Count = 54, Dkim = "fail", Spf = "fail" }
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);

            Assert.AreEqual(27, result.ProviderCalculations[0].PercentDmarcFail);
            Assert.AreEqual(200, result.ProviderCalculations[0].TotalMail);
        }

        [Test]
        public void CreateMapsProviderDkim()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider", Count = 46, Dkim = "pass", DkimPassCount = 0 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 48, Dkim = "pass", DkimPassCount = 1 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 52, Dkim = "fail", DkimPassCount = 0 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 54, Dkim = "fail", DkimPassCount = 1 }
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);

            Assert.AreEqual(26, result.ProviderCalculations[0].PercentDkimAuthFail);
            Assert.AreEqual(27, result.ProviderCalculations[0].PercentDkimMisaligned);
            Assert.AreEqual(200, result.ProviderCalculations[0].TotalMail);
        }

        [Test]
        public void CreateMapsProviderSpf()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider", Count = 46, Spf = "pass", SpfPassCount = 0 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 48, Spf = "pass", SpfPassCount = 1 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 52, Spf = "fail", SpfPassCount = 0 },
                new NormalisedRecord { HostProvider = "testProvider", Count = 54, Spf = "fail", SpfPassCount = 1 }
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);

            Assert.AreEqual(26, result.ProviderCalculations[0].PercentSpfAuthFail);
            Assert.AreEqual(27, result.ProviderCalculations[0].PercentSpfMisaligned);
            Assert.AreEqual(200, result.ProviderCalculations[0].TotalMail);
        }

        [Test]
        public void CreateMapsTotalMixedQuarantineOrReject()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider1", P = "quarantine", Pct = 0, Count = 18 },
                new NormalisedRecord { HostProvider = "testProvider1", P = "quarantine", Pct = 1, Count = 19},
                new NormalisedRecord { HostProvider = "testProvider1", P = "quarantine", Pct = 99, Count = 20},
                new NormalisedRecord { HostProvider = "testProvider1", P = "quarantine", Pct = 100, Count = 21},
                new NormalisedRecord { HostProvider = "testProvider1", P = "none", Pct = 99, Count = 22},
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);

            Assert.AreEqual(39, result.TotalMixedQuarantineOrReject);
        }

        [Test]
        public void CreateMapsProviderAliases()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> 
            {
                new NormalisedRecord { HostProvider = "testProvider", Domain = "testDomain", Count = 17 }
            };

            A.CallTo(() => _providerAliasProvider.GetProviderAlias("testProvider", "testDomain")).Returns("testProviderAlias");

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual("testProviderAlias", result.ProviderCalculations[0].Name);
        }

        [Test]
        public void ProvidersWithTenOrLessEmailsShouldNotContribute()
        {
            List<NormalisedRecord> records = new List<NormalisedRecord> {
                new NormalisedRecord { HostProvider = "testProvider1", P = "reject", Count = 2 },
                new NormalisedRecord { HostProvider = "testProvider2", P = "quarantine", Count = 11},
                new NormalisedRecord { HostProvider = "testProvider3", P = "none", Count = 10 },
            };

            ConfigurationEvaluationObject result = _configurationEvaluationObjectFactory.Create(records, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.ProviderCalculations.Count);
        }
    }
}
