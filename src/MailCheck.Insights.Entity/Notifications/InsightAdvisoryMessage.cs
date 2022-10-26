using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Entity.Notifications
{
    public class InsightAdvisoryMessage : Message
    {
        public InsightAdvisoryMessage(string id, List<AdvisoryMessage> messages) : base(id)
        {
            Messages = messages;
        }
        public List<AdvisoryMessage> Messages { get; }
    }
}
