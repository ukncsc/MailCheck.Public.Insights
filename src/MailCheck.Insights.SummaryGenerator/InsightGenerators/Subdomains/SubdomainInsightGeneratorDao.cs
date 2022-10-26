using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.Contracts.Subdomains;
using MySql.Data.MySqlClient;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains
{
    public interface ISubdomainInsightGeneratorDao
    {
        Task<SubdomainsRawData> GetRawData(string id, DateTime startDate, DateTime endDate);
    }

    public class SubdomainInsightGeneratorDao : ISubdomainInsightGeneratorDao
    {
        private IDatabase _database;

        public SubdomainInsightGeneratorDao(IDatabase database)
        {
            _database = database;
        }

        public async Task<SubdomainsRawData> GetRawData(string id, DateTime startDate, DateTime endDate)
        {
            using (DbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = SubdomainsInsightGeneratorResources.GetLast30DaysRawData;
                    command.Parameters.Add(new MySqlParameter("@reverseDomain", DomainNameUtils.ReverseDomainName(id)));
                    command.Parameters.Add(new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")));
                    command.Parameters.Add(new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd")));
                    
                    command.Prepare();

                    string p = string.Empty;
                    string sp = string.Empty;

                    List<NormalisedRecord> subdomains = new List<NormalisedRecord>();

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            p = reader.IsDBNull(0) ? null : reader.GetString(0);
                            sp = reader.IsDBNull(1) ? null : reader.GetString(1);
                        }

                        reader.NextResult();

                        while (await reader.ReadAsync())
                        {
                            subdomains.Add(new NormalisedRecord
                            {
                                Domain = reader.IsDBNull(0) ? null : reader.GetString(0),
                                Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                P = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Spf = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Dkim = reader.IsDBNull(4) ? null : reader.GetString(4),
                                EffectiveDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5)
                            });
                        }
                    }

                    connection.Close();

                    return new SubdomainsRawData(id, p, sp, subdomains);
                }
            }
        }
    }
}
