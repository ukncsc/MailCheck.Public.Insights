namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public class ProviderRules
    {
        public ProviderAlias[] ProviderAliases { get; set; }
    }

    public class ProviderAlias
    {
        public string Provider { get; set; }
        public string Mapping { get; set; }
    }
}