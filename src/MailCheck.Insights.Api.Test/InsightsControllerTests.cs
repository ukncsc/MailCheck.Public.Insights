using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Insights.Api.Controllers;
using MailCheck.Insights.Api.Dao;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.Api.Test
{
    [TestFixture]
    public class InsightsControllerTests
    {
        private InsightsController _insightsController;
        private IInsightsService _insightsService;

        [SetUp]
        public void Setup()
        {
            _insightsService = A.Fake<IInsightsService>();
            _insightsController = new InsightsController(_insightsService, A.Fake<ILogger<InsightsController>>());
        }

        [Test]
        public async Task GetInsightsReturnsResultFromService()
        {
            InsightsResponseModel responseModelFromService = new InsightsResponseModel();
            A.CallTo(() => _insightsService.GetInsights("testDomain")).Returns(Task.FromResult(responseModelFromService));

            ActionResult<InsightsResponseModel> result = await _insightsController.GetInsights("testDomain");

            Assert.AreSame(responseModelFromService, result.Value);
        }

        [Test]
        public async Task GetInsightsReturns404WhenServiceReturnsNull()
        {
            A.CallTo(() => _insightsService.GetInsights("testDomain")).Returns(Task.FromResult((InsightsResponseModel)null));

            ActionResult<InsightsResponseModel> result = await _insightsController.GetInsights("testDomain");

            Assert.AreSame(typeof(NotFoundResult), result.Result.GetType());
        }

        [Test]
        public async Task GetInsightSummariesReturnsResultFromService()
        {
            InsightsSummaryResponseModel responseModelFromService = new InsightsSummaryResponseModel();
            A.CallTo(() => _insightsService.GetInsightSummaries("testDomain")).Returns(Task.FromResult(responseModelFromService));

            ActionResult<InsightsSummaryResponseModel> result = await _insightsController.GetInsightSummaries("testDomain");

            Assert.AreSame(responseModelFromService, result.Value);
        }

        [Test]
        public async Task GetInsightSummariesReturns404WhenServiceReturnsNull()
        {
            A.CallTo(() => _insightsService.GetInsightSummaries("testDomain")).Returns(Task.FromResult((InsightsSummaryResponseModel)null));

            ActionResult<InsightsSummaryResponseModel> result = await _insightsController.GetInsightSummaries("testDomain");

            Assert.AreSame(typeof(NotFoundResult), result.Result.GetType());
        }
    }
}