using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.DataSavers.Subdomains
{
    public interface ISubdomainDataFactory
    {
        SubdomainData Create(AggregateReportRecordEnriched source);
    }

    public class SubdomainDataFactory : ISubdomainDataFactory
    {
        public SubdomainData Create(AggregateReportRecordEnriched source)
        {
            bool spfFailed = source.Spf != DmarcResult.pass;
            bool dkimFailed = source.Dkim != DmarcResult.pass;

            string domain = DomainNameUtils.ToCanonicalDomainName(source.HeaderFrom ?? source.DomainFrom);
            string reverseDomain = DomainNameUtils.ReverseDomainName(domain);
            bool dispositionQuarantineOrReject = source.Disposition == Policy.quarantine || source.Disposition == Policy.reject;
            bool dispositionNone = !dispositionQuarantineOrReject;

            bool isBlockListOrFailedReverseDns = 
                source.HostName == "Unknown" ||
                source.HostName == "Mismatch" ||
                source.EndUserBlockListCount > 0 ||
                source.BounceReflectorBlockListCount > 0 ||
                source.EndUserNetworkBlockListCount > 0 ||
                source.HijackedNetworkBlockListCount > 0 ||
                source.ProxyBlockListCount > 0 ||
                source.SpamSourceBlockListCount > 0 ||
                source.SuspiciousNetworkBlockListCount > 0 ||
                source.MalwareBlockListCount > 0;

            SubdomainData subdomainData = new SubdomainData
            {
                Domain = domain,
                ReverseDomain = reverseDomain,
                EffectiveDate = source.EffectiveDate.Date,
                RecordId = source.RecordId,
                AllTrafficCount = source.Count,
                DkimOrSpfFailedNoneCount = (spfFailed || dkimFailed) && dispositionNone ? source.Count : 0,
                DkimOrSpfFailedQuarantineOrRejectCount = (spfFailed || dkimFailed) && dispositionQuarantineOrReject ? source.Count : 0,
                DkimAndSpfFailedNoneCount = spfFailed && dkimFailed && dispositionNone ? source.Count : 0,
                DkimAndSpfFailedQuarantineOrRejectCount = spfFailed && dkimFailed && dispositionQuarantineOrReject ? source.Count : 0,
                BlockListOrFailedReverseDnsCount = isBlockListOrFailedReverseDns ? source.Count : 0
            };

            return subdomainData;
        }
    }
}