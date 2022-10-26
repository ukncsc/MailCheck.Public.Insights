using System;
using System.Collections.Generic;
using System.Text;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Subdomains;
using MailCheck.Insights.SummaryGenerator.Config;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules
{
    public interface ISubdomainInsightAdvisoryMessageGenerator
    {
        NamedAdvisory Generate(Guid id, string name, MessageType type, string title, string note, List<SubdomainsData> data,
            Func<SubdomainsData, int> getPercentage);
    }

    public class SubdomainInsightAdvisoryMessageGenerator : ISubdomainInsightAdvisoryMessageGenerator
    {
        private readonly ISummaryGeneratorConfig _config;

        public SubdomainInsightAdvisoryMessageGenerator(ISummaryGeneratorConfig config)
        {
            _config = config;
        }

        public NamedAdvisory Generate(Guid id, string name, MessageType type, string title, string markDown,
            List<SubdomainsData> data, Func<SubdomainsData, int> getPercentage)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("| Subdomain | Email | % failing DMARC | Link |");
            sb.AppendLine("|---|---|---|---|");

            for (int i = 0; i < data.Count; i++)
            {
                SubdomainsData subdomainsData = data[i];

                if (i == 150)
                {
                    break;
                }

                string row =
                    $"|{subdomainsData.Domain}|{subdomainsData.AllTrafficCount}|{getPercentage(subdomainsData)}%|[View]({_config.MailCheckUrlPrefix}/app/domain-security/{subdomainsData.Domain}/dmarc-insights)|";
                sb.AppendLine(row);
            }

            sb.AppendLine("---");
            
            if (data.Count > 150)
            {
                sb.AppendLine(string.Format(SubdomainsRulesMarkdownResource.MoreThan150RecordMessage, data.Count - 150));
                sb.AppendLine("---");
            }

            return new NamedAdvisory(id, name, type, title, string.Format(markDown, sb.ToString()));
        }
    }
}
