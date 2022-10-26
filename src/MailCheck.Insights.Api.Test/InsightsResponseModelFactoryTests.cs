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
    public class InsightsResponseModelFactoryTests
    {
        private InsightsResponseModelFactory _insightsResponseModelFactory;

        [SetUp]
        public void Setup()
        {
            _insightsResponseModelFactory = new InsightsResponseModelFactory();
        }

        [Test]
        public void CreateGroupsAndMapsAllProperties()
        {
            InsightEntityState source = new InsightEntityState
            {
                CalculatedAt = new DateTime(2000, 01, 01),
                StartDate = new DateTime(2000, 01, 02),
                EndDate = new DateTime(2000, 01, 03),
                Insights = new List<Insight>
                {
                    new Insight(InsightType.Abuse,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname1", MessageType.error, "text1", "markdown1"),
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname2", MessageType.info, "text2", "markdown2"),
                        }
                    ),
                    new Insight(InsightType.Configuration,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname3", MessageType.warning, "text3", "markdown3")
                        }
                    ),
                    new Insight(InsightType.Subdomains,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname4", MessageType.error, "text4", "markdown4")
                        }
                    ),
                    new Insight(InsightType.Abuse,
                        new List<NamedAdvisory>
                        {
                            new NamedAdvisory(Guid.Empty, "mailcheck.insights.testname5", MessageType.warning, "text5", "markdown5")
                        }
                    )
                }
            };

            InsightsResponseModel result = _insightsResponseModelFactory.Create(source);

            Assert.AreEqual(3, result.InsightGroups.Count);

            InsightsResponseModel.InsightGroup abuseGroup = result.InsightGroups[0];
            Assert.AreEqual("Abuse", abuseGroup.InsightType);
            Assert.AreEqual(3, abuseGroup.InsightAdvisories.Count);
            Assert.AreEqual("text1", abuseGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("text2", abuseGroup.InsightAdvisories[1].Text);
            Assert.AreEqual("text5", abuseGroup.InsightAdvisories[2].Text);
            Assert.AreEqual("markdown1", abuseGroup.InsightAdvisories[0].MarkDown);
            Assert.AreEqual("markdown2", abuseGroup.InsightAdvisories[1].MarkDown);
            Assert.AreEqual("markdown5", abuseGroup.InsightAdvisories[2].MarkDown);
            Assert.AreEqual("error", abuseGroup.InsightAdvisories[0].AdvisoryType);
            Assert.AreEqual("info", abuseGroup.InsightAdvisories[1].AdvisoryType);
            Assert.AreEqual("warning", abuseGroup.InsightAdvisories[2].AdvisoryType);

            InsightsResponseModel.InsightGroup configurationGroup = result.InsightGroups[1];
            Assert.AreEqual("Configuration", configurationGroup.InsightType);
            Assert.AreEqual(1, configurationGroup.InsightAdvisories.Count);
            Assert.AreEqual("text3", configurationGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("markdown3", configurationGroup.InsightAdvisories[0].MarkDown);
            Assert.AreEqual("warning", configurationGroup.InsightAdvisories[0].AdvisoryType);

            InsightsResponseModel.InsightGroup subdomainGroup = result.InsightGroups[2];
            Assert.AreEqual("Subdomains", subdomainGroup.InsightType);
            Assert.AreEqual(1, subdomainGroup.InsightAdvisories.Count);
            Assert.AreEqual("text4", subdomainGroup.InsightAdvisories[0].Text);
            Assert.AreEqual("markdown4", subdomainGroup.InsightAdvisories[0].MarkDown);
            Assert.AreEqual("error", subdomainGroup.InsightAdvisories[0].AdvisoryType);

            Assert.AreEqual(new DateTime(2000, 01, 01), result.CalculatedAt);
            Assert.AreEqual(new DateTime(2000, 01, 02), result.StartDate);
            Assert.AreEqual(new DateTime(2000, 01, 03), result.EndDate);
        }
    }
}