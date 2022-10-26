using MailCheck.Insights.Contracts;
using System;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using MySqlHelper = MailCheck.Common.Data.Util.MySqlHelper;

namespace MailCheck.Insights.Entity.Dao
{
    public interface IInsightEntityDao
    {
        Task Save(InsightEntityState state);
        Task<int> Delete(string domain);
        Task<InsightEntityState> Get(string domain);
    }

    public class InsightEntityDao : IInsightEntityDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;

        public InsightEntityDao(IConnectionInfoAsync connectionInfoAsync)
        {
            _connectionInfoAsync = connectionInfoAsync;
        }

        public async Task<int> Delete(string domain)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            return await MySqlHelper.ExecuteNonQueryAsync(connectionString, InsightEntityDaoResources.DeleteEntity,
                new MySqlParameter("id", domain));
        }

        public async Task<InsightEntityState> Get(string domain)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string state = (string)await MySqlHelper.ExecuteScalarAsync(connectionString, InsightEntityDaoResources.SelectEntity,
                new MySqlParameter("domain", domain));

            return state == null
                ? null
                : JsonConvert.DeserializeObject<InsightEntityState>(state);
        }

        public async Task Save(InsightEntityState state)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string serializedState = JsonConvert.SerializeObject(state);

            int rowsAffected = await MySqlHelper.ExecuteNonQueryAsync(connectionString, InsightEntityDaoResources.InsertEntity,
                new MySqlParameter("domain", state.Id.ToLower()),
                new MySqlParameter("version", state.Version),
                new MySqlParameter("state", serializedState));

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException(
                    $"Didn't update InsightEvaluationComplete because version {state.Version} has already been persisted.");
            }
        }
    }
}
