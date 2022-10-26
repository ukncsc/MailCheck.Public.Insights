using System;
using MailCheck.Common.Contracts.Advisories;

namespace MailCheck.Insights.Contracts
{
    public class NamedAdvisory : AdvisoryMessage
    {
        public string Name { get; }

        public NamedAdvisory(Guid id, string name, MessageType messageType, string text, string markDown)
            : base(id, messageType, text, markDown)
        {
            Name = name;
        }
    }
}
