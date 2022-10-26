using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Contracts.Findings;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Processors.Notifiers;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Entity.Config;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.Entity.Notifiers
{
    public class FindingsChangedNotifier : IChangeNotifier
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly IInsightEntityConfig _insightEntityConfig;
        private readonly IFindingsChangedNotifier _findingsChangedCalculator;
        private readonly ILogger<FindingsChangedNotifier> _log;

        public FindingsChangedNotifier(IMessageDispatcher dispatcher, IInsightEntityConfig insightEntityConfig,
            IFindingsChangedNotifier findingsChangedCalculator, ILogger<FindingsChangedNotifier> log)
        {
            _dispatcher = dispatcher;
            _insightEntityConfig = insightEntityConfig;
            _findingsChangedCalculator = findingsChangedCalculator;
            _log = log;
        }

        public void Handle(string domain, List<NamedAdvisory> currentMessages, List<NamedAdvisory> newMessages)
        {
            FindingsChanged findingsChanged = _findingsChangedCalculator.Process(domain, "INSIGHTS",
                ExtractFindingsFromMessages(domain, currentMessages?.OfType<NamedAdvisory>().ToList() ?? new List<NamedAdvisory>()),
                ExtractFindingsFromMessages(domain, newMessages?.OfType<NamedAdvisory>().ToList() ?? new List<NamedAdvisory>()));

            if (findingsChanged.Added?.Count > 0 || findingsChanged.Sustained?.Count > 0 || findingsChanged.Removed?.Count > 0)
            {
                _log.LogInformation($"Dispatching FindingsChanged for {domain}: {findingsChanged.Added?.Count} findings added, {findingsChanged.Sustained?.Count} findings sustained, {findingsChanged.Removed?.Count} findings removed");
                _dispatcher.Dispatch(findingsChanged, _insightEntityConfig.SnsTopicArn);
            }
            else
            { 
                _log.LogInformation($"No Findings to dispatch for {domain}");
            }
        }

        private List<Finding> ExtractFindingsFromMessages(string domain, List<NamedAdvisory> rootMessages)
        {
            List<Finding> findings = rootMessages.Select(msg => new Finding
            {
                Name = msg.Name,
                SourceUrl = $"https://{_insightEntityConfig.WebUrl}/app/domain-security/{domain}/dmarc-insights",
                Title = msg.Text,
                EntityUri = $"domain:{domain}",
                Severity = AdvisoryMessageTypeToFindingSeverityMapping[msg.MessageType]
            }).ToList();

            return findings;
        }

        internal static readonly Dictionary<MessageType, string> AdvisoryMessageTypeToFindingSeverityMapping = new Dictionary<MessageType, string>
        {
            [MessageType.info] = "Informational",
            [MessageType.warning] = "Advisory",
            [MessageType.error] = "Urgent",
            [MessageType.success] = "Positive"
        };
    }
}