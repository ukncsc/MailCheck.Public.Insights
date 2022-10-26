using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Insights.Contracts.Configuration;
using MailCheck.Insights.DataSaver.Contract;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.DataSaver.DataSavers.Configuration
{
    public class ConfigurationDataSaver : IDataSaver
    {
        private readonly IDatabase _database;
        private readonly IConfigurationDataFactory _configurationDataFactory;
        private readonly ILogger<ConfigurationDataSaver> _log;

        public ConfigurationDataSaver(IDatabase database, IConfigurationDataFactory configurationDataFactory, ILogger<ConfigurationDataSaver> log)
        {
            _database = database;
            _configurationDataFactory = configurationDataFactory;
            _log = log;
        }

        public async Task SaveData(AggregateReportRecordEnriched source)
        {
            // TODO pending outcome of https://ukncsc.atlassian.net/browse/DMARC-4812 just return
            await Task.CompletedTask;

            //int? affectedRows;
            //ConfigurationData configurationData = _configurationDataFactory.Create(source);
            //using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            //{
            //    affectedRows = await connection.ExecuteAsync(ConfigurationDataSaverResources.InsertConfigurationData, configurationData);
            //}

            //_log.LogInformation(affectedRows > 0
            //    ? $"Configuration Data for {configurationData.Domain} saved."
            //    : $"Configuration Data not saved for {configurationData.Domain}. Record id {configurationData.RecordId} on effective date {configurationData.EffectiveDate} already processed.");
        }
    }
}
