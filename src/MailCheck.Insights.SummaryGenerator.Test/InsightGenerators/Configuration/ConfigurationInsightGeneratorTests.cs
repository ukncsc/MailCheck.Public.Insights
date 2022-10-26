using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Configuration
{
    [TestFixture]
    public class ConfigurationInsightGeneratorTests
    {
        private ConfigurationInsightGenerator _configurationInsightGenerator;
        private IConfigurationInsightGeneratorDao _configurationInsightGeneratorDao;
        private IEvaluator<ConfigurationEvaluationObject> _evaluator;
        private IConfigurationEvaluationObjectFactory _configurationEvaluationObjectFactory;
        private IExcludedTrafficFilter _excludedTrafficFilter;

        [SetUp]
        public void SetUp()
        {
            _configurationInsightGeneratorDao = A.Fake<IConfigurationInsightGeneratorDao>();
            _evaluator = A.Fake<IEvaluator<ConfigurationEvaluationObject>>();
            _configurationEvaluationObjectFactory = A.Fake<IConfigurationEvaluationObjectFactory>();
            _excludedTrafficFilter = A.Fake<IExcludedTrafficFilter>();

            _configurationInsightGenerator = new ConfigurationInsightGenerator(_evaluator, _configurationInsightGeneratorDao, _configurationEvaluationObjectFactory, _excludedTrafficFilter, A.Fake<ILogger<ConfigurationInsightGenerator>>());
        }

        [Test]
        public async Task GenerateInsightsReturnsInsights()
        {
            List<NormalisedRecord> normalisedRecordsFromDatabase = new List<NormalisedRecord>();
            A.CallTo(() => _configurationInsightGeneratorDao.GetRaw("testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02))).Returns(normalisedRecordsFromDatabase);

            List<NormalisedRecord> filteredRecords = new List<NormalisedRecord> { new NormalisedRecord() };
            A.CallTo(() => _excludedTrafficFilter.Filter(normalisedRecordsFromDatabase)).Returns(filteredRecords);

            ConfigurationEvaluationObject configurationEvaluationObjectFromFactory = new ConfigurationEvaluationObject();
            A.CallTo(() => _configurationEvaluationObjectFactory.Create(filteredRecords, "testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02))).Returns(configurationEvaluationObjectFromFactory);

            NamedAdvisory advisoryMessage = new NamedAdvisory(Guid.Empty, "mailcheck.insights.configInsufficientData", MessageType.info, string.Empty, string.Empty);
            EvaluationResult<ConfigurationEvaluationObject> resultFromEvaluator = new EvaluationResult<ConfigurationEvaluationObject>(configurationEvaluationObjectFromFactory,
                    advisoryMessage);
            A.CallTo(() => _evaluator.Evaluate(configurationEvaluationObjectFromFactory)).Returns(resultFromEvaluator);

            Insight result = await _configurationInsightGenerator.GenerateInsights("testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.Advisories.Count);
            Assert.AreSame(advisoryMessage, result.Advisories[0]);
        }

        [Test]
        public async Task GenerateInsightsAdvisesWhenNoData()
        {
            List<NormalisedRecord> normalisedRecordsFromDatabase = new List<NormalisedRecord>();
            A.CallTo(() => _configurationInsightGeneratorDao.GetRaw("testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02))).Returns(normalisedRecordsFromDatabase);

            List<NormalisedRecord> filteredRecords = new List<NormalisedRecord>();
            A.CallTo(() => _excludedTrafficFilter.Filter(normalisedRecordsFromDatabase)).Returns(filteredRecords);

            Insight result = await _configurationInsightGenerator.GenerateInsights("testDomain", new DateTime(2001, 01, 01), new DateTime(2002, 02, 02));

            Assert.AreEqual(1, result.Advisories.Count);
            Assert.AreEqual("We have insufficient DMARC reporting data over the last 30 days to produce configuration insights for testDomain", result.Advisories[0].Text);
        }

    }
}