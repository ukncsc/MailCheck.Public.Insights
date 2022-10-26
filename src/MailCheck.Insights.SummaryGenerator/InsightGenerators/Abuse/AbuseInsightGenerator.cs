using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Factories;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse
{
    public class AbuseInsightGenerator : IInsightGenerator
    {
        private readonly IAbuseInsightGeneratorDao _dao;
        private readonly ILogger<AbuseInsightGenerator> _log;
        private readonly IEvaluator<AbuseEvaluationObject> _insightGenerator;
        private readonly ISummaryGeneratorConfig _config;
        private readonly IFlaggedTrafficDataFactory _flaggedTrafficDataFactory;

        private readonly Guid _noDataGuid = new Guid("2c33cb59-5b6f-48e7-ab2c-be151270c401");

        public AbuseInsightGenerator(IAbuseInsightGeneratorDao dao, ILogger<AbuseInsightGenerator> log, 
            IEvaluator<AbuseEvaluationObject> insightGenerator, ISummaryGeneratorConfig config, IFlaggedTrafficDataFactory flaggedTrafficDataFactory)
        {
            _dao = dao;
            _log = log;
            _insightGenerator = insightGenerator;
            _config = config;
            _flaggedTrafficDataFactory = flaggedTrafficDataFactory;
        }

        public async Task<Insight> GenerateInsights(string id, DateTime startDate, DateTime endDate)
        {
            _log.LogInformation($"Begin - GenerateInsights of type Abuse for {id}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<RawAbuseData> rawAbuseData = await _dao.GetRawData(id, startDate, endDate);
            List<RawSubdomainAbuseData> rawSubdomainAbuseData = await _dao.GetRawSubdomainData(id, startDate, endDate);

            _log.LogInformation($"Raw data retrieved for {id} after {stopwatch.ElapsedMilliseconds} ms");

            List<FlaggedTrafficData> flaggedTrafficData = _flaggedTrafficDataFactory.Create(rawAbuseData);
            List<FlaggedTrafficData> flaggedSubdomainTrafficData = _flaggedTrafficDataFactory.Create(rawSubdomainAbuseData);

            Insight result;
            if ((flaggedTrafficData == null && flaggedSubdomainTrafficData == null) 
               || (flaggedTrafficData.Count == 0 && flaggedSubdomainTrafficData.Count == 0))
            {
                result = new Insight(InsightType.Abuse, new List<NamedAdvisory>
                {
                    new NamedAdvisory(_noDataGuid, "mailcheck.insights.noAbuseData", MessageType.info, $"We have insufficient DMARC reporting data over the last 30 days to produce abuse insights for {id}", null)
                });
            }
            else
            {
                _log.LogInformation($"Abuse data found for {id} after {stopwatch.ElapsedMilliseconds} ms");

                AbuseEvaluationObject evaluationObject = new AbuseEvaluationObject
                {
                    Domain = id,
                    FlaggedTrafficData = flaggedTrafficData,
                    FlaggedSubdomainTrafficData = flaggedSubdomainTrafficData,
                    StartDate = startDate,
                    EndDate = endDate,
                    Url = _config.MailCheckUrlPrefix
                };

                EvaluationResult<AbuseEvaluationObject> abuseInsights = await _insightGenerator.Evaluate(evaluationObject);

                result = new Insight(InsightType.Abuse, abuseInsights.AdvisoryMessages.Select(adv => (NamedAdvisory)adv).ToList());
            }

            stopwatch.Stop();
            _log.LogInformation($"Complete - GenerateInsights of type Abuse after {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
    }
}
