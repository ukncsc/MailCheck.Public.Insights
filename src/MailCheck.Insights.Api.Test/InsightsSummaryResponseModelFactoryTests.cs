using System;
using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Api.Service;
using MailCheck.Insights.Contracts;
using NUnit.Framework;

namespace MailCheck.Insights.Api.Test
{
    [TestFixture]
    public class InsightsSummaryResponseModelFactoryTests
    {
        private InsightsSummaryResponseModelFactory _insightsSummaryResponseModelFactory;

        [SetUp]
        public void Setup()
        {
            _insightsSummaryResponseModelFactory = new InsightsSummaryResponseModelFactory();
        }

        [Test]
        public void CreateGroupsAndMapsAllProperties()
        {
            InsightEntityState source = new InsightEntityState
            {
                Insights = new List<Insight>
                {
                    new Insight(InsightType.Abuse,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname1", MessageType.error, "text1", string.Empty),
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname2", MessageType.info, "text2", string.Empty),
                        }
                    ),
                    new Insight(InsightType.Configuration,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname3", MessageType.warning, "text3", string.Empty)
                        }
                    ),
                    new Insight(InsightType.Subdomains,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname4", MessageType.error, "text4", string.Empty)
                        }
                    ),
                    new Insight(InsightType.Abuse,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname5", MessageType.warning, "text5", string.Empty)
                        }
                    )
                }
            };

            InsightsSummaryResponseModel result = _insightsSummaryResponseModelFactory.Create(source, true);

            Assert.True(result.HasReportingData);

            Assert.AreEqual(3, result.InsightGroups.Count);

            InsightsSummaryResponseModel.InsightGroup abuseGroup = result.InsightGroups[0];
            Assert.AreEqual("Abuse", abuseGroup.InsightType);
            Assert.AreEqual(3, abuseGroup.InsightAdvisories.Count);
            Assert.AreEqual("text1", abuseGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("text2", abuseGroup.InsightAdvisories[1].Text);
            Assert.AreEqual("text5", abuseGroup.InsightAdvisories[2].Text);
            Assert.AreEqual("error", abuseGroup.InsightAdvisories[0].AdvisoryType);
            Assert.AreEqual("info", abuseGroup.InsightAdvisories[1].AdvisoryType);
            Assert.AreEqual("warning", abuseGroup.InsightAdvisories[2].AdvisoryType);

            InsightsSummaryResponseModel.InsightGroup configurationGroup = result.InsightGroups[1];
            Assert.AreEqual("Configuration", configurationGroup.InsightType);
            Assert.AreEqual(1, configurationGroup.InsightAdvisories.Count);
            Assert.AreEqual("text3", configurationGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("warning", configurationGroup.InsightAdvisories[0].AdvisoryType);

            InsightsSummaryResponseModel.InsightGroup subdomainGroup = result.InsightGroups[2];
            Assert.AreEqual("Subdomains", subdomainGroup.InsightType);
            Assert.AreEqual(1, subdomainGroup.InsightAdvisories.Count);
            Assert.AreEqual("text4", subdomainGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("error", subdomainGroup.InsightAdvisories[0].AdvisoryType);
        }

        [Test]
        public void CreateReturnsNullIfNoReportingDataOrEntity()
        {
            InsightsSummaryResponseModel result = _insightsSummaryResponseModelFactory.Create(null, false);

            Assert.Null(result);
        }

        [Test]
        public void CreateReturnsFlagIfEntityButNoReportingData()
        {
            InsightsSummaryResponseModel result = _insightsSummaryResponseModelFactory.Create(new InsightEntityState(), false);

            Assert.False(result.HasReportingData);
            Assert.Null(result.InsightGroups);
        }

        [Test]
        public void CreateReturnsFlagIfReportingDataButNoEntity()
        {
            InsightsSummaryResponseModel result = _insightsSummaryResponseModelFactory.Create(null, true);

            Assert.True(result.HasReportingData);
            Assert.Null(result.InsightGroups);
        }
    }
}