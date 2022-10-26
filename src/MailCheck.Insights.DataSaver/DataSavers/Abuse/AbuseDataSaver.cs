using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Insights.Contracts.Abuse;
using MailCheck.Insights.DataSaver.Contract;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.DataSaver.DataSavers.Abuse
{
    public class AbuseDataSaver : IDataSaver
    {
        private readonly ILogger<AbuseDataSaver> _log;
        private readonly IAbuseDataSaverFactory _abuseDataFactory;
        private readonly IDatabase _database;

        public AbuseDataSaver(ILogger<AbuseDataSaver> log, IAbuseDataSaverFactory abuseDataFactory, IDatabase database)
        {
            _log = log;
            _abuseDataFactory = abuseDataFactory;
            _database = database;
        }

        public async Task SaveData(AggregateReportRecordEnriched message)
        {
            int? affectedRows;
            AbuseData abuseData = _abuseDataFactory.Create(message);

            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                affectedRows = await connection.ExecuteAsync(AbuseDataSaverResources.InsertAbuseData, abuseData);
            }

            _log.LogInformation(affectedRows > 0
                ? $"Abuse Data for {abuseData.Domain} saved with {abuseData.Flagged} of {abuseData.Count} flagged."
                : $"Abuse Data not saved for {abuseData.Domain}. Record id {abuseData.RecordId} on effective date {abuseData.EffectiveDate} already processed.");
        }
    }
}