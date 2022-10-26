using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Raw;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public class ConfigurationInsightGenerator : IInsightGenerator
    {
        private readonly IConfigurationInsightGeneratorDao _dao;
        private readonly IEvaluator<ConfigurationEvaluationObject> _insightGenerator;
        private readonly IConfigurationEvaluationObjectFactory _configurationEvaluationObjectFactory;
        private readonly IExcludedTrafficFilter _filter;
        private readonly ILogger<ConfigurationInsightGenerator> _log;


        private readonly Guid _noDataGuid = new Guid("c8908d02-f382-47ab-9505-a0bd376855b1");

        public ConfigurationInsightGenerator(
            IEvaluator<ConfigurationEvaluationObject> insightGenerator,
            IConfigurationInsightGeneratorDao dao, 
            IConfigurationEvaluationObjectFactory configurationEvaluationObjectFactory, 
            IExcludedTrafficFilter filter,
            ILogger<ConfigurationInsightGenerator> log)
        {
            _insightGenerator = insightGenerator;
            _dao = dao;
            _log = log;
            _filter = filter;
            _configurationEvaluationObjectFactory = configurationEvaluationObjectFactory;
        }

        public async Task<Insight> GenerateInsights(string id, DateTime startDate, DateTime endDate)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _log.LogInformation($"Begin - GenerateInsights of type Configuration for {id}");

            List<NormalisedRecord> rawRecords = await _dao.GetRaw(id, startDate, endDate);
            var aa = rawRecords.Sum(x => x.Count);
            _log.LogInformation($"Raw data retrieved for {id} after {stopwatch.ElapsedMilliseconds} ms");

            List<NormalisedRecord> filteredRecords = _filter.Filter(rawRecords);
            var bb = filteredRecords.Sum(x => x.Count);

            Insight result;
            if (filteredRecords.Count == 0)
            {
                result = new Insight(InsightType.Configuration, new List<NamedAdvisory>
                {
                    new NamedAdvisory(_noDataGuid, "mailcheck.insights.configInsufficientData", MessageType.info, $"We have insufficient DMARC reporting data over the last 30 days to produce configuration insights for {id}", null)
                });
            }
            else
            {
                _log.LogInformation($"Configuration data found for {id} after {stopwatch.ElapsedMilliseconds} ms");

                ConfigurationEvaluationObject evaluationObject = _configurationEvaluationObjectFactory.Create(filteredRecords, id, startDate, endDate);
                EvaluationResult<ConfigurationEvaluationObject> configurationInsights = await _insightGenerator.Evaluate(evaluationObject);

                result = new Insight(InsightType.Configuration, configurationInsights.AdvisoryMessages.Select(adv => (NamedAdvisory)adv).ToList());
            }

            stopwatch.Stop();
            _log.LogInformation($"Complete - GenerateInsights of type Configuration for {id} in {stopwatch.ElapsedMilliseconds} ms");

            return result;

        }
    }
}