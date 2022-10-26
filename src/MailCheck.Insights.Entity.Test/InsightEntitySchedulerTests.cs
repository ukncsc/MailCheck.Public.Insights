using FakeItEasy;
using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using MailCheck.Insights.Entity.Config;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailCheck.Insights.Entity.Test
{
    [TestFixture]
    public class InsightEntitySchedulerTests
    {
        private const string Id = "abc.com";
        
        private IInsightEntityConfig _config;
        private IMessageDispatcher _dispatcher;
        private IInsightEntityScheduler _scheduler;
        private ILogger<InsightEntityScheduler> _log;
        private IClock _clock;

        [SetUp]
        public void Setup()
        {
            _config = A.Fake<IInsightEntityConfig>();
            _dispatcher = A.Fake<IMessageDispatcher>();
            _clock = A.Fake<IClock>();
            _log = A.Fake<ILogger<InsightEntityScheduler>>();
            _scheduler = new InsightEntityScheduler(_dispatcher, _config, _clock, _log);
        }
        
        [Test]
        public void HandlingDomainDeletedDispatchesDeleteScheduledReminder()
        {
             _scheduler.Handle(new DomainDeleted(Id));

            A.CallTo(() => _dispatcher.Dispatch(A<DeleteScheduledReminder>.That.Matches(_ =>
                _.ResourceId == Id && _.Service == "Insights"), A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void HandlingDomainCreatedDispatchesCreateScheduledReminder()
        {
            _scheduler.Handle(new DomainCreated(Id, "test@test.com", DateTime.Now));

            A.CallTo(() => _dispatcher.Dispatch(A<CreateScheduledReminder>.That.Matches(_ =>
                _.ResourceId == Id && _.Service == "Insights"), A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}
