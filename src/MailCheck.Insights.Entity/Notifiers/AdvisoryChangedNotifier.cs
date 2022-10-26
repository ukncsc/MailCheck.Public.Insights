using System.Collections.Generic;
using System.Linq;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Entity.Config;
using MailCheck.Insights.Entity.Notifications;
using MailCheck.Insights.Entity.Notifiers;
using Microsoft.Extensions.Logging;
using AdvisoryMessage = MailCheck.Common.Contracts.Advisories.AdvisoryMessage;

namespace MailCheck.EmailSecurity.Entity.Entity.Notifiers
{
    public class AdvisoryChangedNotifier : IChangeNotifier
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly IInsightEntityConfig _insightEntityConfig;
        private readonly IEqualityComparer<AdvisoryMessage> _messageEqualityComparer;
        private readonly ILogger<AdvisoryChangedNotifier> _log;

        public AdvisoryChangedNotifier(IMessageDispatcher dispatcher, IInsightEntityConfig insightEntityConfig,
            IEqualityComparer<AdvisoryMessage> messageEqualityComparer, ILogger<AdvisoryChangedNotifier> log)
        {
            _dispatcher = dispatcher;
            _insightEntityConfig = insightEntityConfig;
            _messageEqualityComparer = messageEqualityComparer;
            _log = log;
        }

        public void Handle(string domain, List<NamedAdvisory> currentMessages, List<NamedAdvisory> newMessages)
        {
            currentMessages = currentMessages ?? new List<NamedAdvisory>();
            newMessages = newMessages ?? new List<NamedAdvisory>();

            List<AdvisoryMessage> addedMessages =
                newMessages.Except(currentMessages, _messageEqualityComparer).ToList();
            if (addedMessages.Any())
            {
                InsightAdvisoryAdded advisoryAdded = new InsightAdvisoryAdded(domain,
                    addedMessages);
                _dispatcher.Dispatch(advisoryAdded, _insightEntityConfig.SnsTopicArn);
                _log.LogInformation($"Dispatching {addedMessages.Count} added messages for {domain}");
            }

            List<AdvisoryMessage> removedMessages =
                currentMessages.Except(newMessages, _messageEqualityComparer).ToList();
            if (removedMessages.Any())
            {
                InsightAdvisoryRemoved advisoryRemoved = new InsightAdvisoryRemoved(domain,
                    removedMessages);
                _dispatcher.Dispatch(advisoryRemoved, _insightEntityConfig.SnsTopicArn);
                _log.LogInformation($"Dispatching {removedMessages.Count} removed messages for {domain}");
            }

            List<AdvisoryMessage> sustainedMessages =
                currentMessages.Intersect(newMessages, _messageEqualityComparer).ToList();
            if (sustainedMessages.Any())
            {
                InsightAdvisorySustained advisorySustained = new InsightAdvisorySustained(domain,
                    sustainedMessages);
                _dispatcher.Dispatch(advisorySustained, _insightEntityConfig.SnsTopicArn);
                _log.LogInformation($"Dispatching {sustainedMessages.Count} sustained messages for {domain}");
            }
        }
    }
}