using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test
{
    [TestFixture]
    public class SummaryGeneratorTests
    {
        private InsightEvaluationPendingHandler _insightEvaluationPendingHandler;
        private List<IInsightGenerator> _summaryGenerators;
        private IMessageDispatcher _messageDispatcher;
        private ISummaryGeneratorConfig _summaryGeneratorConfig;
        private IClock _clock;

        [SetUp]
        public void Setup()
        {
            _messageDispatcher = A.Fake<IMessageDispatcher>();
            _summaryGeneratorConfig = A.Fake<ISummaryGeneratorConfig>();
            _summaryGenerators = new List<IInsightGenerator>();
            _clock = A.Fake<IClock>();

            _insightEvaluationPendingHandler = new InsightEvaluationPendingHandler(_messageDispatcher, _summaryGeneratorConfig, _summaryGenerators, _clock, A.Fake<ILogger<InsightEvaluationPendingHandler>>());
        }

        [Test]
        public async Task HandleDispatchesResultsOfInsightGenerators()
        {
            IInsightGenerator fakeInsightGenerator = A.Fake<IInsightGenerator>();
            Insight testInsights = new Insight(InsightType.Abuse, new List<NamedAdvisory> { new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.test", MessageType.error, "test", "test") });
            A.CallTo(() => fakeInsightGenerator.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights);
            _summaryGenerators.Add(fakeInsightGenerator);

            A.CallTo(() => _clock.GetDateTimeUtc()).Returns(DateTime.UnixEpoch);
            A.CallTo(() => _summaryGeneratorConfig.SnsTopicArn).Returns("testSnsTopicArn");

            await _insightEvaluationPendingHandler.Handle(new InsightEvaluationPending("test.com"));

            Expression<Func<InsightEvaluationComplete, bool>> expression =
                x => x.Id == "test.com" &&
                     x.CalculatedAt == DateTime.UnixEpoch &&
                     x.Insights.Count == 1 &&
                     x.Insights[0].Type == InsightType.Abuse &&
                     x.Insights[0].Advisories[0].MessageType == MessageType.error;

            A.CallTo(() =>
                    _messageDispatcher.Dispatch(A<InsightEvaluationComplete>.That.Matches(expression),
                        "testSnsTopicArn"))
                .MustHaveHappened();
        }

        [Test]
        public async Task InsightNotAddedToListOfInsightsWhenHasNoAdvisories()
        {
            IInsightGenerator fakeInsightGenerator = A.Fake<IInsightGenerator>();
            Insight testInsights = new Insight(InsightType.Abuse, new List<NamedAdvisory>());
            A.CallTo(() => fakeInsightGenerator.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights);
            _summaryGenerators.Add(fakeInsightGenerator);

            A.CallTo(() => _clock.GetDateTimeUtc()).Returns(DateTime.UnixEpoch);
            A.CallTo(() => _summaryGeneratorConfig.SnsTopicArn).Returns("testSnsTopicArn");

            await _insightEvaluationPendingHandler.Handle(new InsightEvaluationPending("test.com"));

            A.CallTo(() => _messageDispatcher.Dispatch(A<InsightEvaluationComplete>.That.Matches(_ => _.Insights.Count == 0), A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task MultipleInsightGeneratorsOneWithNoAdvisoriesIsntAdded()
        {
            IInsightGenerator fakeInsightGenerator1 = A.Fake<IInsightGenerator>();
            IInsightGenerator fakeInsightGenerator2 = A.Fake<IInsightGenerator>();
            IInsightGenerator fakeInsightGenerator3 = A.Fake<IInsightGenerator>();
            Insight testInsights1 = new Insight(InsightType.Abuse, new List<NamedAdvisory>());
            Insight testInsights2 = new Insight(InsightType.Configuration, new List<NamedAdvisory> { new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.test", MessageType.error, "test", "test") });
            Insight testInsights3 = new Insight(InsightType.Subdomains, new List<NamedAdvisory> { new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.test", MessageType.error, "test", "test") });
            A.CallTo(() => fakeInsightGenerator1.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights1);
            A.CallTo(() => fakeInsightGenerator2.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights2);
            A.CallTo(() => fakeInsightGenerator3.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights3);
            _summaryGenerators.Add(fakeInsightGenerator1);
            _summaryGenerators.Add(fakeInsightGenerator2);
            _summaryGenerators.Add(fakeInsightGenerator3);

            A.CallTo(() => _clock.GetDateTimeUtc()).Returns(DateTime.UnixEpoch);
            A.CallTo(() => _summaryGeneratorConfig.SnsTopicArn).Returns("testSnsTopicArn");

            await _insightEvaluationPendingHandler.Handle(new InsightEvaluationPending("test.com"));

            A.CallTo(() => _messageDispatcher.Dispatch(A<InsightEvaluationComplete>.That.Matches(_ => _.Insights.Count == 2), A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task InsightNotAddedToListOfInsightsWhenNullAdvisories()
        {
            IInsightGenerator fakeInsightGenerator = A.Fake<IInsightGenerator>();
            Insight testInsights = new Insight(InsightType.Abuse, null);
            A.CallTo(() => fakeInsightGenerator.GenerateInsights("test.com", A<DateTime>._, A<DateTime>._)).Returns(testInsights);
            _summaryGenerators.Add(fakeInsightGenerator);

            A.CallTo(() => _clock.GetDateTimeUtc()).Returns(DateTime.UnixEpoch);
            A.CallTo(() => _summaryGeneratorConfig.SnsTopicArn).Returns("testSnsTopicArn");

            await _insightEvaluationPendingHandler.Handle(new InsightEvaluationPending("test.com"));

            A.CallTo(() => _messageDispatcher.Dispatch(A<InsightEvaluationComplete>.That.Matches(_ => _.Insights.Count == 0), A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}