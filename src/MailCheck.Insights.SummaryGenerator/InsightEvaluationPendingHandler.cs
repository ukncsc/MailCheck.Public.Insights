using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.SummaryGenerator
{
    public class InsightEvaluationPendingHandler :IHandle<InsightEvaluationPending>
    {
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly ISummaryGeneratorConfig _summaryGeneratorConfig;
        private readonly IEnumerable<IInsightGenerator> _insightGenerators;
        private readonly IClock _clock;
        private readonly ILogger<InsightEvaluationPendingHandler> _log;

        public InsightEvaluationPendingHandler(
            IMessageDispatcher messageDispatcher, 
            ISummaryGeneratorConfig summaryGeneratorConfig, 
            IEnumerable<IInsightGenerator> insightGenerators, 
            IClock clock, 
            ILogger<InsightEvaluationPendingHandler> log)
        {
            _messageDispatcher = messageDispatcher;
            _summaryGeneratorConfig = summaryGeneratorConfig;
            _insightGenerators = insightGenerators;
            _clock = clock;
            _log = log;
        }

        public async Task Handle(InsightEvaluationPending message)
        {
            _log.LogInformation($"Begin: processing insight summary data for {message.Id}");
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DateTime timestamp = _clock.GetDateTimeUtc();

            List<Insight> insights = new List<Insight>();

            DateTime endDate = timestamp.Date.Add(_summaryGeneratorConfig.WaitForDataPeriod.Negate());
            DateTime startDate = endDate.Add(_summaryGeneratorConfig.SummaryPeriod.Negate());

            foreach (IInsightGenerator generator in _insightGenerators)
            {
                Insight genInsight = await generator.GenerateInsights(message.Id, startDate, endDate);
                if (genInsight.Advisories?.Count > 0)
                {
                    insights.Add(genInsight);
                }
            }

            InsightEvaluationComplete insightEvaluationComplete = new InsightEvaluationComplete(message.Id,
                timestamp, insights, startDate, endDate);

            _messageDispatcher.Dispatch(insightEvaluationComplete, _summaryGeneratorConfig.SnsTopicArn);

            stopwatch.Stop();
            _log.LogInformation($"Complete: processing insight summary data for {message.Id} in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
