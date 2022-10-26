using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Entity.Config;
using MailCheck.Insights.Entity.Dao;
using MailCheck.Insights.Entity.Notifiers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.Entity.Test
{
    [TestFixture]
    public class InsightEvaluationCompletedHandlerTests
    {
        private const string Id = "abc.com";

        private IInsightEntityDao _dao;
        private IInsightEntityConfig _config;
        private IMessageDispatcher _dispatcher;
        private IInsightEntityScheduler _scheduler;
        private ILogger<InsightEntityHandler> _log;
        private IChangeNotifiersComposite _changeNotifiersComposite;
        private InsightEntityHandler _handler;

        [SetUp]
        public void Setup()
        {
            _dao = A.Fake<IInsightEntityDao>();
            _config = A.Fake<IInsightEntityConfig>();
            _dispatcher = A.Fake<IMessageDispatcher>();
            _scheduler = A.Fake<IInsightEntityScheduler>();
            _log = A.Fake<ILogger<InsightEntityHandler>>();
            _changeNotifiersComposite = A.Fake<IChangeNotifiersComposite>();
            _handler = new InsightEntityHandler(_dao, _dispatcher, _config, _scheduler, _log, _changeNotifiersComposite);
        }

        [Test]
        public async Task HandlingDomainCreatedCreatesDomainIfEntityDoesNotExist()
        {
            A.CallTo(() => _dao.Get(Id)).Returns<InsightEntityState>(null);

            await _handler.Handle(new DomainCreated(Id, "test@test.com", DateTime.Now));

            A.CallTo(() => _dao.Save(A<InsightEntityState>.That.Matches(_ =>
                _.Id == Id && _.Version == 1))).MustHaveHappenedOnceExactly();

            A.CallTo(() => _scheduler.Handle(A<DomainCreated>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task HandlingDomainDeletedDeletesStateFromDb()
        {
            await _handler.Handle(new DomainDeleted(Id));

            A.CallTo(() => _dao.Delete(Id)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _scheduler.Handle(A<DomainDeleted>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task HandlingInsightEvaluationCompleteMessage()
        {
            InsightEntityState state = new InsightEntityState
            {
                Id = Id,
                CalculatedAt = new DateTime(1),
                Insights = new List<Insight>(),
                StartDate = new DateTime(2),
                EndDate = new DateTime(3),
                Version = 1
            };

            A.CallTo(() => _dao.Get(Id)).Returns(state);

            await _handler.Handle(new InsightEvaluationComplete(Id, new DateTime(4), new List<Insight>()
            {
                new Insight(InsightType.Abuse, new List<NamedAdvisory> { new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.testname1", MessageType.error, "test", "test") })
            }, new DateTime(5), new DateTime(6)));

            A.CallTo(() => _changeNotifiersComposite.Handle(Id,
                A<List<NamedAdvisory>>.That.Matches(x => x.Count == 0),
                A<List<NamedAdvisory>>.That.Matches(x => x.Count == 1))).MustHaveHappenedOnceExactly();

            A.CallTo(() => _dao.Save(A<InsightEntityState>.That.Matches(_ =>
                _.Id == Id &&
                _.Version == 2 &&
                _.Insights.Count == 1 &&
                _.CalculatedAt == new DateTime(4) &&
                _.StartDate == new DateTime(5) &&
                _.EndDate == new DateTime(6)))).MustHaveHappenedOnceExactly();
        }
    }
}