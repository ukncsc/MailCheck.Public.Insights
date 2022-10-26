using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.SummaryGenerator.Config;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration
{
    public interface IConfigurationEvaluationObjectFactory
    {
        ConfigurationEvaluationObject Create(List<NormalisedRecord> recordsOrderedByDate, string domain, DateTime startDate, DateTime endDate);
    }

    public class ConfigurationEvaluationObjectFactory : IConfigurationEvaluationObjectFactory
    {
        private readonly ISummaryGeneratorConfig _config;
        private readonly IProviderAliasProvider _providerAliasProvider;

        public ConfigurationEvaluationObjectFactory(ISummaryGeneratorConfig config, IProviderAliasProvider providerAliasProvider)
        {
            _config = config;
            _providerAliasProvider = providerAliasProvider;
        }

        public ConfigurationEvaluationObject Create(List<NormalisedRecord> recordsOrderedByDate, string domain, DateTime startDate, DateTime endDate)
        {
            int totalCount = recordsOrderedByDate.Sum(record => record.Count);
            int totalDmarcFail = recordsOrderedByDate.Where(x => x.Dkim != "pass" && x.Spf != "pass").Sum(x => x.Count);
            int totalNone = recordsOrderedByDate.Where(x => x.P == "none").Sum(x => x.Count);
            int totalQuarantineOrReject = recordsOrderedByDate.Where(x => x.P == "quarantine" || x.P == "reject").Sum(x => x.Count); 
            int totalMixedQuarantineOrReject = recordsOrderedByDate.Where(x => (x.P == "quarantine" || x.P == "reject") && x.Pct >= 1 && x.Pct <= 99).Sum(x => x.Count);

            IEnumerable<IGrouping<string, NormalisedRecord>> providerConfigurationData = recordsOrderedByDate
                .GroupBy(record => record.HostProvider, record => record, StringComparer.InvariantCultureIgnoreCase);

            List<ProviderCalculation> providerCalculations = new List<ProviderCalculation>();

            foreach (IGrouping<string, NormalisedRecord> providerData in providerConfigurationData)
            {
                int totalMail = providerData.Sum(x => x.Count);

                if (totalMail <= 10) continue;

                decimal percentOfAllTraffic = totalMail * 100M / totalCount;
                if (percentOfAllTraffic < 1) continue;

                int providerNone = providerData.Where(x => x.P == "none").Sum(x => x.Count);
                int providerQuarantineOrReject = providerData.Where(x => x.P == "quarantine" || x.P == "reject").Sum(x => x.Count);
                int providerDmarcFail = providerData.Where(x => x.Dkim != "pass" && x.Spf != "pass").Sum(x => x.Count);
                int providerDkimAuthFail = providerData.Where(x => x.Dkim != "pass" && x.DkimPassCount == 0).Sum(x => x.Count);
                int providerDkimMisaligned = providerData.Where(x => x.Dkim != "pass" && x.DkimPassCount > 0).Sum(x => x.Count);
                int providerSpfAuthFail = providerData.Where(x => x.Spf != "pass" && x.SpfPassCount == 0).Sum(x => x.Count);
                int providerSpfMisaligned = providerData.Where(x => x.Spf != "pass" && x.SpfPassCount > 0).Sum(x => x.Count);

                ProviderCalculation providerCalculation = new ProviderCalculation
                {
                    Name = _providerAliasProvider.GetProviderAlias(providerData.Key, domain),
                    TotalMail = totalMail,
                    PercentNone = providerNone * 100M / totalMail,
                    PercentDmarcFail = providerDmarcFail * 100M / totalMail,
                    PercentDkimAuthFail = providerDkimAuthFail * 100M / totalMail,
                    PercentDkimMisaligned = providerDkimMisaligned * 100M / totalMail,
                    PercentQuarantineOrReject = providerQuarantineOrReject * 100M / totalMail,
                    PercentSpfAuthFail = providerSpfAuthFail * 100M / totalMail,
                    PercentSpfMisaligned = providerSpfMisaligned * 100M / totalMail,
                };

                providerCalculations.Add(providerCalculation);
            }

            ConfigurationEvaluationObject result = new ConfigurationEvaluationObject
            {
                Domain = domain,
                StartDate = startDate,
                EndDate = endDate,
                Url = _config.MailCheckUrlPrefix,
                ProviderCalculations = providerCalculations,
                TotalTraffic = totalCount,
                PercentFailingDmarc = totalDmarcFail * 100M / totalCount,
                PercentNone = totalNone * 100M / totalCount,
                PercentQuarantineOrReject = totalQuarantineOrReject * 100M / totalCount,
                LatestPolicy = recordsOrderedByDate.LastOrDefault()?.P,
                TotalMixedQuarantineOrReject = totalMixedQuarantineOrReject
            };

            return result;
        }
    }
}