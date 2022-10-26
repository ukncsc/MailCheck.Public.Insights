using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Entity.Notifications
{
    public class InsightAdvisoryRemoved : InsightAdvisoryMessage
    {
        public InsightAdvisoryRemoved(string id, List<AdvisoryMessage> messages) : base(id, messages)
        {
        }
    }
}
