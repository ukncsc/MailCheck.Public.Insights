using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration
{
    [TestFixture]
    public class ProviderAliasProviderTests
    {
        private ProviderAliasProvider _providerDetailsProvider;

        [SetUp]
        public void SetUp()
        {
            _providerDetailsProvider = new ProviderAliasProvider();
        }

        [Test]
        public void GetProviderAliasShouldReturnAlias()
        {
            string alias = _providerDetailsProvider.GetProviderAlias("testProvider", "testDomain");

            Assert.AreEqual("testAlias", alias);
        }

        [Test]
        public void GetProviderAliasShouldReturnSourceValueIfNotFound()
        {
            string alias = _providerDetailsProvider.GetProviderAlias("unknownProvider", "testDomain");

            Assert.AreEqual("unknownProvider", alias);
        }

        [Test]
        public void GetProviderAliasShouldAppendDetailIfDomainMatchesProvider()
        {
            string alias = _providerDetailsProvider.GetProviderAlias("testDomain", "testDomain");

            Assert.AreEqual("testDomain (i.e. your on-premise email system, gateway or relay)", alias);
        }
    }
}