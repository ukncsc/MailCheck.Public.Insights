using System.Collections.Generic;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Entity.Notifiers
{
    public interface IChangeNotifier
    {
        void Handle(string domain, List<NamedAdvisory> currentMessages, List<NamedAdvisory> newMessages);
    }
}