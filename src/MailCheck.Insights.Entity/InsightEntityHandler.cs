using MailCheck.Common.Contracts.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Entity.Dao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Insights.Entity.Config;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Messaging.Common.Exception;
using MailCheck.Insights.Entity.Notifiers;
using System.Linq;

namespace MailCheck.Insights.Entity
{
    public class InsightEntityHandler :
        IHandle<DomainCreated>, IHandle<DomainDeleted>,
        IHandle<InsightEvaluationComplete>, IHandle<InsightsScheduledReminder>
    {
        private readonly ILogger<InsightEntityHandler> _log;
        private readonly IInsightEntityDao _entityDao;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IInsightEntityConfig _config;
        private readonly IInsightEntityScheduler _insightEntityScheduler;
        private readonly IChangeNotifiersComposite _changeNotifiersComposite;

        public InsightEntityHandler(IInsightEntityDao entityDao, IMessageDispatcher dispatcher,
            IInsightEntityConfig config, IInsightEntityScheduler insightEntityScheduler,
            ILogger<InsightEntityHandler> log, IChangeNotifiersComposite changeNotifiersComposite)
        {
            _entityDao = entityDao;
            _log = log;
            _dispatcher = dispatcher;
            _config = config;
            _insightEntityScheduler = insightEntityScheduler;
            _changeNotifiersComposite = changeNotifiersComposite;
        }

        public async Task Handle(DomainCreated message)
        {
            string domain = message.Id.ToLower();

            InsightEntityState state = await _entityDao.Get(domain);

            if (state != null)
            {
                _log.LogInformation($"Ignoring {nameof(DomainCreated)} as Insight Entity already exists for {domain}.");
                return;
            }

            state = new InsightEntityState
            {
                Id = domain,
                CalculatedAt = null,
                Insights = new List<Insight>(),
                StartDate = null,
                EndDate = null,
                Version = 0
            };

            state.Version++;
            await _entityDao.Save(state);
            _log.LogInformation($"Created Insight Entity for {domain}.");

            _insightEntityScheduler.Handle(message);
        }

        public async Task Handle(DomainDeleted message)
        {
            string domain = message.Id.ToLower();
            int rows = await _entityDao.Delete(domain);
            if (rows == 1)
            {
                _log.LogInformation($"Deleted InsightEntity for: {domain}.");
            }
            else
            {
                _log.LogInformation($"InsightEntity already deleted for: {domain}.");
            }

            _insightEntityScheduler.Handle(message);

        }

        public async Task Handle(InsightEvaluationComplete message)
        {
            string domain = message.Id.ToLower();

            InsightEntityState state = await LoadState(domain, typeof(InsightEvaluationComplete).Name);

            _changeNotifiersComposite.Handle(
                domain,
                state.Insights.SelectMany(x => x.Advisories).ToList(),
                message.Insights.SelectMany(x => x.Advisories).ToList());

            state.Insights = message.Insights;
            state.StartDate = message.StartDate;
            state.EndDate = message.EndDate;
            state.CalculatedAt = message.CalculatedAt;
            state.Version++;

            await _entityDao.Save(state);
            _log.LogInformation($"Updated Insight entity with id: {domain}.");

            _insightEntityScheduler.Handle(message);
        }

        public Task Handle(InsightsScheduledReminder message)
        {
            string domain = message.ResourceId.ToLower();

            InsightEvaluationPending evaluationPending = new InsightEvaluationPending(domain);
            _dispatcher.Dispatch(evaluationPending, _config.SnsTopicArn);
            _log.LogInformation($"Dispatched Insight Evaluation Pending with id: {domain}.");

            return Task.CompletedTask;
        }

        private async Task<InsightEntityState> LoadState(string id, string messageType)
        {
            InsightEntityState state = await _entityDao.Get(id);

            if (state == null)
            {
                _log.LogInformation($"Ignoring {messageType} as Insights Entity does not exist for {id}.");
                throw new MailCheckException(
                    $"Cannot handle event {messageType} as Insights Entity doesnt exists for {id}.");
            }

            return state;
        }
    }
}