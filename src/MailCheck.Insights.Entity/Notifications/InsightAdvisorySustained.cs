using System.Collections.Generic;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Entity.Notifications
{
    public class InsightAdvisorySustained : InsightAdvisoryMessage
    {
        public InsightAdvisorySustained(string id, List<AdvisoryMessage> messages) : base(id, messages)
        {
        }
    }
}
