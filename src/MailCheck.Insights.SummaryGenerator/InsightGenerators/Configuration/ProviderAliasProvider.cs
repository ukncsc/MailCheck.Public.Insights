using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public interface IProviderAliasProvider
    {
        string GetProviderAlias(string provider, string domain);
    }

    public class ProviderAliasProvider : IProviderAliasProvider
    {
        private readonly Dictionary<string, string> _providerAlias;

        public ProviderAliasProvider()
        {
            string sources = File.ReadAllText("ProviderRules.json");
            ProviderRules providerRules = JsonConvert.DeserializeObject<ProviderRules>(sources);

            _providerAlias = providerRules.ProviderAliases.ToDictionary(x => x.Provider, y => y.Mapping);
        }

        public string GetProviderAlias(string provider, string domain)
        {
            if (provider == domain)
            {
                return  $"{provider} (i.e. your on-premise email system, gateway or relay)";
            }

            if (_providerAlias.ContainsKey(provider))
            {
                return _providerAlias[provider];
            }

            return provider;
        }
    }
}