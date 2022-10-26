using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.Insights.Contracts
{
    public class InsightEvaluationPending : Message
    {
        public InsightEvaluationPending(string id) : base(id)
        {
        }
    }
}