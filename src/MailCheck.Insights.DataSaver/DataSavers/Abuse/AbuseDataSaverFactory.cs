using MailCheck.Common.Util;
using MailCheck.Insights.Contracts.Abuse;
using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.DataSavers.Abuse
{
    public interface IAbuseDataSaverFactory
    {
        AbuseData Create(AggregateReportRecordEnriched source);
    }

    public class AbuseDataFactory : IAbuseDataSaverFactory
    {
        public AbuseData Create(AggregateReportRecordEnriched message)
        {
            string domain = DomainNameUtils.ToCanonicalDomainName(message.HeaderFrom ?? message.DomainFrom);

            AbuseData abuseData = new AbuseData
            {
                EffectiveDate = message.EffectiveDate.Date,
                Domain = domain,
                ReverseDomain = DomainNameUtils.ReverseDomainName(domain),
                Count = message.Count,
                Flagged = 0,
                P = message.P.ToString(),
                Pct = message.Pct,
                RecordId = message.RecordId
            };

            if (message.HostName == "Unknown" ||
                message.HostName == "Mismatch" ||
                message.EndUserBlockListCount > 0 ||
                message.BounceReflectorBlockListCount > 0 ||
                message.EndUserNetworkBlockListCount > 0 ||
                message.HijackedNetworkBlockListCount > 0 ||
                message.ProxyBlockListCount > 0 ||
                message.SpamSourceBlockListCount > 0 ||
                message.SuspiciousNetworkBlockListCount > 0 ||
                message.MalwareBlockListCount > 0)
            {
                abuseData.Flagged += abuseData.Count;
            }

            return abuseData;
        }
    }
}