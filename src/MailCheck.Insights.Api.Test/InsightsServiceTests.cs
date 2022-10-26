using System;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Api.Dao;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Api.Service;
using MailCheck.Insights.Contracts;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.Api.Test
{
    [TestFixture]
    public class InsightsServiceTests
    {
        private InsightsService _insightsService;
        private IInsightsDao _insightsDao;
        private IInsightsResponseModelFactory _insightsResponseModelFactory;
        private IInsightsSummaryResponseModelFactory _insightsSummaryResponseModelFactory;
        private IMessagePublisher _messagePublisher;
        private IInsightsApiConfig _insightsApiConfig;
        private ILogger<InsightsService> _log;

        [SetUp]
        public void Setup()
        {
            _insightsDao = A.Fake<IInsightsDao>();
            _insightsResponseModelFactory = A.Fake<IInsightsResponseModelFactory>();
            _insightsSummaryResponseModelFactory = A.Fake<IInsightsSummaryResponseModelFactory>();
            _messagePublisher = A.Fake<IMessagePublisher>();
            _insightsApiConfig = A.Fake<IInsightsApiConfig>();
            _log = A.Fake<ILogger<InsightsService>>();

            _insightsService = new InsightsService(_insightsDao, _insightsResponseModelFactory, _insightsSummaryResponseModelFactory, _messagePublisher, _insightsApiConfig, _log);
        }

        [Test]
        public async Task GetInsightsMapsResultFromDao()
        {
            InsightEntityState stateFromDatabase = new InsightEntityState();
            A.CallTo(() => _insightsDao.GetInsights("testDomain")).Returns(Task.FromResult(stateFromDatabase));

            InsightsResponseModel mappedResponseModel = new InsightsResponseModel();
            A.CallTo(() => _insightsResponseModelFactory.Create(stateFromDatabase)).Returns(mappedResponseModel);

            InsightsResponseModel result = await _insightsService.GetInsights("testDomain");

            Assert.AreSame(mappedResponseModel, result);
        }

        [Test]
        public async Task GetInsightsReturnsNullIfNoStateAndNotReported()
        {
            A.CallTo(() => _insightsDao.GetInsights("testDomain")).Returns(Task.FromResult((InsightEntityState)null));
            A.CallTo(() => _insightsDao.GetIsReported("testDomain")).Returns(Task.FromResult(false));

            InsightsResponseModel result = await _insightsService.GetInsights("testDomain");

            Assert.Null(result);
        }

        [Test]
        public async Task GetInsightsReturnsIgnoredIfNoStateAndReported()
        {
            A.CallTo(() => _insightsDao.GetInsights("testDomain")).Returns(Task.FromResult((InsightEntityState)null));
            A.CallTo(() => _insightsDao.GetIsReported("testDomain")).Returns(Task.FromResult(true));

            InsightsResponseModel result = await _insightsService.GetInsights("testDomain");

            Assert.True(result.Ignored);
        }

        [Test]
        public async Task GetInsightSummariesMapsResultFromDao()
        {
            InsightEntityState stateFromDatabase = new InsightEntityState();
            A.CallTo(() => _insightsDao.GetInsights("testDomain")).Returns(Task.FromResult(stateFromDatabase));
            A.CallTo(() => _insightsDao.GetIsReported("testDomain")).Returns(Task.FromResult(true));

            InsightsSummaryResponseModel mappedResponseModel = new InsightsSummaryResponseModel();
            A.CallTo(() => _insightsSummaryResponseModelFactory.Create(stateFromDatabase, true)).Returns(mappedResponseModel);

            InsightsSummaryResponseModel result = await _insightsService.GetInsightSummaries("testDomain");

            Assert.AreSame(mappedResponseModel, result);
        }

        [Test]
        public async Task GetInsightSummariesPublishesDomainMissingIfNoState()
        {
            A.CallTo(() => _insightsDao.GetInsights("testDomain")).Returns(Task.FromResult((InsightEntityState)null));
            A.CallTo(() => _insightsDao.GetIsReported("testDomain")).Returns(Task.FromResult(false));
            A.CallTo(() => _insightsApiConfig.MicroserviceOutputSnsTopicArn).Returns("testSnsTopic");

            InsightsSummaryResponseModel result = await _insightsService.GetInsightSummaries("testDomain");

            A.CallTo(() => _messagePublisher.Publish(A<DomainMissing>.That.Matches(x => x.Id == "testDomain"), "testSnsTopic")).MustHaveHappened();
        }
    }
}