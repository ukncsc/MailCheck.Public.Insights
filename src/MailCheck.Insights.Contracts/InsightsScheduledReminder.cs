using MailCheck.Common.Contracts.Messaging;

namespace MailCheck.Insights.Contracts
{
    public class InsightsScheduledReminder : ScheduledReminder
    {
        public InsightsScheduledReminder(string id, string resourceId)
            : base(id, resourceId)
        {
        }
    }
}
