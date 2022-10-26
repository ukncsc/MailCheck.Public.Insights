using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using System;
using MailCheck.Common.Util;
using MailCheck.Insights.Entity.Config;
using Microsoft.Extensions.Logging;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Entity
{
    public interface IInsightEntityScheduler
    {
        void Handle(DomainCreated message);
        void Handle(DomainDeleted message);
        void Handle(InsightEvaluationComplete message);
    }

    public class InsightEntityScheduler : IInsightEntityScheduler
    {
        private readonly IClock _clock;
        private const string ServiceName = "Insights";
        private readonly ILogger<InsightEntityScheduler> _log;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IInsightEntityConfig _config;

        public InsightEntityScheduler(IMessageDispatcher dispatcher, IInsightEntityConfig config, IClock clock,
            ILogger<InsightEntityScheduler> log)
        {
            _dispatcher = dispatcher;
            _config = config;
            _clock = clock;
            _log = log;
        }

        public void Handle(DomainCreated message)
        {
            string domain = message.Id.ToLower();

            CreateScheduledReminder createScheduledReminder = new CreateScheduledReminder(
                Guid.NewGuid().ToString(),
                ServiceName,
                domain,
                default);

            _dispatcher.Dispatch(createScheduledReminder, _config.SnsTopicArn);

            _log.LogInformation(
                $"Dispatched CreateScheduledReminder message for Domain: {domain} has been to SnsTopic: {_config.SnsTopicArn}");

        }

        public void Handle(DomainDeleted message)
        {
            string domain = message.Id.ToLower();

            DeleteScheduledReminder deleteScheduledReminder = new DeleteScheduledReminder(
                Guid.NewGuid().ToString(),
                ServiceName,
                domain);

            _dispatcher.Dispatch(deleteScheduledReminder, _config.SnsTopicArn);

            _log.LogInformation(
                $"Dispatched DeleteScheduledReminder message for Domain: {domain} has been to SnsTopic: {_config.SnsTopicArn}");
        }

        public void Handle(InsightEvaluationComplete message)
        {
            string domain = message.Id.ToLower();

            Message reminderSuccessful = new ReminderSuccessful(
                Guid.NewGuid().ToString(),
                ServiceName,
                domain,
                _clock.GetDateTimeUtc());

            _dispatcher.Dispatch(reminderSuccessful, _config.SnsTopicArn);

            _log.LogInformation(
                $"Dispatched ReminderSuccessful message for Domain: {domain} has been to SnsTopic: {_config.SnsTopicArn}");
        }
    }
}
