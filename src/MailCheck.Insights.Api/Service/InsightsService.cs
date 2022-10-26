using System.Threading.Tasks;
using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Api.Dao;
using MailCheck.Insights.Api.Domain;
using MailCheck.Insights.Contracts;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.Api.Service
{
    public interface IInsightsService
    {
        Task<InsightsResponseModel> GetInsights(string domain);
        Task<InsightsSummaryResponseModel> GetInsightSummaries(string domain);
    }

    public class InsightsService : IInsightsService
    {
        private readonly IInsightsDao _insightsDao;
        private readonly IInsightsResponseModelFactory _insightsResponseModelFactory;
        private readonly IInsightsSummaryResponseModelFactory _insightsSummaryResponseModelFactory;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IInsightsApiConfig _insightsApiConfig;
        private readonly ILogger<InsightsService> _log;

        public InsightsService(IInsightsDao insightsDao, IInsightsResponseModelFactory insightsResponseModelFactory, IInsightsSummaryResponseModelFactory insightsSummaryResponseModelFactory, IMessagePublisher messagePublisher, IInsightsApiConfig insightsApiConfig, ILogger<InsightsService> log)
        {
            _insightsDao = insightsDao;
            _insightsResponseModelFactory = insightsResponseModelFactory;
            _insightsSummaryResponseModelFactory = insightsSummaryResponseModelFactory;
            _messagePublisher = messagePublisher;
            _insightsApiConfig = insightsApiConfig;
            _log = log;
        }

        public async Task<InsightsResponseModel> GetInsights(string domain)
        {
            InsightEntityState state = await _insightsDao.GetInsights(domain);
            if (state == null)
            {
                bool isReported = await _insightsDao.GetIsReported(domain);
                if (isReported)
                {
                    return new InsightsResponseModel { Ignored = true };
                }
            }

            return state == null ? null : _insightsResponseModelFactory.Create(state);
        }

        public async Task<InsightsSummaryResponseModel> GetInsightSummaries(string domain)
        {
            bool hasReportingData = await _insightsDao.GetIsReported(domain);
            InsightEntityState state = await _insightsDao.GetInsights(domain);

            if (state == null)
            {
                _log.LogInformation($"DMARC Insights do not exist for domain {domain} - publishing DomainMissing");
                await _messagePublisher.Publish(new DomainMissing(domain), _insightsApiConfig.MicroserviceOutputSnsTopicArn);
            }

            InsightsSummaryResponseModel responseModel = _insightsSummaryResponseModelFactory.Create(state, hasReportingData);

            return responseModel;
        }
    }
}
