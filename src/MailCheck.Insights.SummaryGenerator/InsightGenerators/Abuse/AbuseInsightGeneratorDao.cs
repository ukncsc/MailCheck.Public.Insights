using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Common.Util;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse
{
    public interface IAbuseInsightGeneratorDao
    {
        Task<List<RawAbuseData>> GetRawData(string domain, DateTime startDate, DateTime endDate);

        Task<List<RawSubdomainAbuseData>> GetRawSubdomainData(string domain, DateTime startDate, DateTime endDate);
    }

    public class AbuseInsightGeneratorDao : IAbuseInsightGeneratorDao
    {
        private readonly IDatabase _database;

        public AbuseInsightGeneratorDao(IDatabase database)
        {
            _database = database;
        }

        public async Task<List<RawAbuseData>> GetRawData(string domain, DateTime startDate, DateTime endDate)
        {
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                List<RawAbuseData> rawData = new List<RawAbuseData>(
                    await connection.QueryAsync<RawAbuseData>(
                        AbuseInsightGeneratorDaoResources.GetRawData, new
                        {
                            reverseDomain = DomainNameUtils.ReverseDomainName(domain),
                            startDate = startDate.ToString("yyyy-MM-dd"),
                            endDate = endDate.ToString("yyyy-MM-dd")
                        }));

                return rawData;
            }
        }

        public async Task<List<RawSubdomainAbuseData>> GetRawSubdomainData(string domain, DateTime startDate, DateTime endDate)
        {
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                List<RawSubdomainAbuseData> rawData = new List<RawSubdomainAbuseData>(
                    await connection.QueryAsync<RawSubdomainAbuseData>(
                        AbuseInsightGeneratorDaoResources.GetRawSubdomainData, new
                        {
                            reverseDomain = DomainNameUtils.ReverseDomainName(domain),
                            startDate = startDate.ToString("yyyy-MM-dd"),
                            endDate = endDate.ToString("yyyy-MM-dd")
                        }));

                return rawData;
            }
        }
    }
}