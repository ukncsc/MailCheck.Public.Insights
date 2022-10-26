using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Insights.DataSaver.Contract;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.DataSaver.DataSavers.Subdomains
{
    public class SubdomainDataSaver : IDataSaver
    {
        private readonly IDatabase _database;
        private readonly ISubdomainDataFactory _subdomainDataFactory;
        private readonly ILogger<SubdomainDataSaver> _log;

        public SubdomainDataSaver(IDatabase database, ISubdomainDataFactory subdomainDataFactory, ILogger<SubdomainDataSaver> log)
        {
            _database = database;
            _subdomainDataFactory = subdomainDataFactory;
            _log = log;
        }

        public async Task SaveData(AggregateReportRecordEnriched source)
        {
            int? affectedRows;
            SubdomainData subdomainData = _subdomainDataFactory.Create(source);
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                affectedRows = await connection.ExecuteAsync(SubdomainDataSaverResources.InsertSubdomainData, subdomainData);
            }

            _log.LogInformation(affectedRows > 0
                ? $"Subdomain Data for {subdomainData.Domain} saved."
                : $"Subdomain Data not saved for {subdomainData.Domain}. Record id {subdomainData.RecordId} on effective date {subdomainData.EffectiveDate} already processed.");
        }
    }
}