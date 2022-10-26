using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts.Raw;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public interface IConfigurationInsightGeneratorDao
    {
        Task<List<NormalisedRecord>> GetRaw(string id, DateTime startDate, DateTime endDate);
    }

    public class ConfigurationInsightGeneratorDao : IConfigurationInsightGeneratorDao
    {
        private readonly IDatabase _database;

        public ConfigurationInsightGeneratorDao(IDatabase database)
        {
            _database = database;
        }

        public async Task<List<NormalisedRecord>> GetRaw(string id, DateTime startDate, DateTime endDate)
        {
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                List<NormalisedRecord> rawData = new List<NormalisedRecord>(
                    await connection.QueryAsync<NormalisedRecord>(
                        ConfigurationInsightGeneratorResources.GetSummaryDataRaw, new
                        {
                            reverseDomain = DomainNameUtils.ReverseDomainName(id),
                            startDate,
                            endDate
                        }));
                
                return rawData;
            }
        }
    }
}
