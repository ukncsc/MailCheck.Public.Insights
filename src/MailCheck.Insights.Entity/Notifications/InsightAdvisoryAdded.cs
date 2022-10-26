using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Entity.Notifications
{
    public class InsightAdvisoryAdded : InsightAdvisoryMessage
    {
        public InsightAdvisoryAdded(string id, List<AdvisoryMessage> messages) : base(id, messages)
        {
        }
    }
}
