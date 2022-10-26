using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.DataSaver.Contract;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.DataSaver.DataSavers.Raw
{
    public interface IRawDataSaver : IDataSaver
    {
    }

    public class RawDataSaver : IRawDataSaver
    {
        private readonly IDatabase _database;
        private readonly INormalisedRecordFactory _normalisedRecordFactory;
        private readonly ILogger<RawDataSaver> _log;

        public RawDataSaver(IDatabase database, INormalisedRecordFactory normalisedRecordFactory, ILogger<RawDataSaver> log)
        {
            _database = database;
            _normalisedRecordFactory = normalisedRecordFactory;
            _log = log;
        }

        public async Task SaveData(AggregateReportRecordEnriched aggregateReportRecordEnriched)
        {
            int? affectedRows;
            NormalisedRecord normalisedRecord = _normalisedRecordFactory.Create(aggregateReportRecordEnriched);
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                affectedRows = await connection.ExecuteAsync(RawDataSaverResources.InsertRawAggregateReportRecordEnriched, normalisedRecord);
            }
            
            _log.LogInformation(affectedRows > 0
                ? $"Raw Data for {normalisedRecord.Domain} saved."
                : $"Raw Data not saved for {normalisedRecord.Domain}. Record id {normalisedRecord.RecordId} on effective date {normalisedRecord.EffectiveDate} already processed.");
        }
    }
}