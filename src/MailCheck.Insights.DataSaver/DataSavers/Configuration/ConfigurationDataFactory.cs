using System;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Configuration;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.DataSavers.Configuration
{
    public interface IConfigurationDataFactory
    {
        ConfigurationData Create(AggregateReportRecordEnriched source);
    }

    public class ConfigurationDataFactory : IConfigurationDataFactory
    {
        public ConfigurationData Create(AggregateReportRecordEnriched source)
        {
            bool dkimFailed = source.Dkim != DmarcResult.pass;
            bool spfFailed = source.Spf != DmarcResult.pass;
            bool dispositionNone = source.Disposition != Policy.quarantine && source.Disposition != Policy.reject;

            string domain = DomainNameUtils.ToCanonicalDomainName(source.HeaderFrom ?? source.DomainFrom);
            ConfigurationData configurationData = new ConfigurationData
            {
                Domain = domain,
                Policy = source.P.ToString(),
                Provider = ResolveProvider(source),
                HostName = source.HostName,

                Traffic = source.Count,

                DmarcFail = dkimFailed && spfFailed ? source.Count : 0,

                DkimAuthFail = dkimFailed && source.DkimPassCount == 0 ? source.Count : 0,
                DkimMisaligned = dkimFailed && source.DkimPassCount > 0 ? source.Count : 0,

                SpfAuthFail = spfFailed && source.SpfPassCount == 0 ? source.Count : 0,
                SpfMisaligned = spfFailed && source.SpfPassCount > 0 ? source.Count : 0,
            };

            return configurationData;
        }

        private string ResolveProvider(AggregateReportRecordEnriched source)
        {
            if (source.Arc)
            {
                return "ARC-Forwarded";
            }

            if (source.ProxyBlockListCount + source.SuspiciousNetworkBlockListCount + source.HijackedNetworkBlockListCount + source.EndUserNetworkBlockListCount + source.SpamSourceBlockListCount + source.MalwareBlockListCount + source.EndUserBlockListCount + source.BounceReflectorBlockListCount > 0)
            {
                return "Blocklisted";
            }

            return source.HostProvider;
        }
    }
}