using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules
{
    public interface IConfigurationTableFactory
    {
        string Create(List<ProviderCalculation> badProviders, List<string> goodProviders, ConfigurationEvaluationObject source);
    }

    public class ConfigurationTableFactory : IConfigurationTableFactory
    {
        public string Create(List<ProviderCalculation> badProviders, List<string> goodProviders, ConfigurationEvaluationObject source)
        {
            StringBuilder goodProvidersList = new StringBuilder();

            foreach (string goodProvider in goodProviders)
            {
                string goodProviderItem = $"- {goodProvider}";
                goodProvidersList.AppendLine(goodProviderItem);
            }

            string goodProvidersText = goodProviders.Count > 0 ? $"### Well configured systems that are passing DMARC.{Environment.NewLine}{Environment.NewLine}{goodProvidersList}{Environment.NewLine}---{Environment.NewLine}" : "";

            bool containsGoogle = false;
            StringBuilder badProvidersTable = new StringBuilder();
            badProvidersTable.AppendLine("| Provider | Dmarc Fail | SPF Auth Fail | SPF Misaligned | DKIM Auth Fail | DKIM Misaligned |");
            badProvidersTable.AppendLine("|---|---|---|---|---|---|");
            foreach (ProviderCalculation badProvider in badProviders)
            {
                decimal percentDmarcFail = Math.Round(badProvider.PercentDmarcFail, 0);
                decimal percentSpfAuthFail = Math.Round(badProvider.PercentSpfAuthFail, 0);
                decimal percentSpfMisaligned = Math.Round(badProvider.PercentSpfMisaligned, 0);
                decimal percentDkimAuthFail = Math.Round(badProvider.PercentDkimAuthFail, 0);
                decimal percentDkimMisaligned = Math.Round(badProvider.PercentDkimMisaligned, 0);
                string providerName = badProvider.Name;
                if (providerName == "Google")
                {
                    containsGoogle = true;
                    providerName = "Google *";
                }

                string badProviderRow = $"|{providerName}|{percentDmarcFail}%|{percentSpfAuthFail}%|{percentSpfMisaligned}%|{percentDkimAuthFail}%|{percentDkimMisaligned}%|";
                badProvidersTable.AppendLine(badProviderRow);
            }

            badProvidersTable.AppendLine("---");

            string googleInfo = "";
            if (containsGoogle)
            {
                googleInfo = "\\* If you don't use Google, this traffic is likely to be auto-forwarding via Gmail etc. Configure DKIM on your email senders to reduce the negative effects of auto-forwarding.";
            }

            string markdown = string.Format(ConfigurationTableFactoryResources.ConfigurationTableMarkdown,
                goodProvidersText, badProvidersTable, source.Domain, source.StartDate, source.EndDate, source.Url, googleInfo);

            return markdown;
        }
    }
}
