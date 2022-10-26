using System;
using System.Collections.Generic;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Entity.Notifiers
{
    public interface IChangeNotifiersComposite : IChangeNotifier
    {
    }

    public class ChangeNotifiersComposite : IChangeNotifiersComposite
    {
        private readonly IEnumerable<IChangeNotifier> _notifiers;

        public ChangeNotifiersComposite(IEnumerable<IChangeNotifier> notifiers)
        {
            _notifiers = notifiers;
        }

        public void Handle(string domain, List<NamedAdvisory> currentMessages, List<NamedAdvisory> newMessages)
        {
            foreach (IChangeNotifier changeNotifier in _notifiers)
            {
                changeNotifier.Handle(domain, currentMessages, newMessages);
            }
        }
    }
}
