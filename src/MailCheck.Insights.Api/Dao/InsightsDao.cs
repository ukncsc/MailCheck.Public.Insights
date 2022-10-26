using System.Data;
using System.Threading.Tasks;
using Dapper;
using MailCheck.Common.Data;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using Newtonsoft.Json;

namespace MailCheck.Insights.Api.Dao
{
    public interface IInsightsDao
    {
        Task<InsightEntityState> GetInsights(string domain);
        Task<bool> GetIsReported(string domain);
    }

    public class InsightsDao : IInsightsDao
    {
        private readonly IDatabase _database;

        public InsightsDao(IDatabase database)
        {
            _database = database;
        }

        public async Task<InsightEntityState> GetInsights(string domain)
        {
            using (IDbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                string state = await connection.QuerySingleOrDefaultAsync<string>(InsightsResources.GetInsights, new { Domain = domain });
                
                if (state == null) return null;

                InsightEntityState result = JsonConvert.DeserializeObject<InsightEntityState>(state);

                return result;
            }
        }

        public async Task<bool> GetIsReported(string domain)
        {
            using (IDbConnection connection = await _database.CreateAndOpenConnectionAsync())
            {
                bool isReported = await connection.QuerySingleOrDefaultAsync<bool>(InsightsResources.GetIsReported, new { ReverseDomain = DomainNameUtils.ReverseDomainName(domain) });
                
                return isReported;
            }
        }
    }
}