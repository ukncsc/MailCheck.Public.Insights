using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers;
using MailCheck.Insights.DataSaver.DataSavers.Raw;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.DataSaver
{
    public class AggregateReportRecordEnrichedHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IEnumerable<IDataSaver> _dataSavers;
        private readonly IRawDataSaver _rawDataSaver;
        private readonly ILogger<AggregateReportRecordEnrichedHandler> _log;

        public AggregateReportRecordEnrichedHandler(
            IEnumerable<IDataSaver> dataSavers, 
            IRawDataSaver rawDataSaver, 
            ILogger<AggregateReportRecordEnrichedHandler> log)
        {
            _dataSavers = dataSavers;
            _log = log;
            _rawDataSaver = rawDataSaver;
        }

        public async Task Handle(AggregateReportRecordEnriched message)
        {
            string domain = message.HeaderFrom ?? message.DomainFrom;

            using (_log.BeginScope(new Dictionary<string, string>
            {
                ["Domain"] = domain
            }))
            {
                _log.LogInformation($"Begin: processing AggregateReportRecordEnriched for {domain}");

                IEnumerable<Task> dataSaverTasks = _dataSavers.Select(x => x.SaveData(message));

                await Task.WhenAll(dataSaverTasks);

                await _rawDataSaver.SaveData(message);

                _log.LogInformation($"Complete: processing AggregateReportRecordEnriched for {domain}");
            }
        }
    }
}
