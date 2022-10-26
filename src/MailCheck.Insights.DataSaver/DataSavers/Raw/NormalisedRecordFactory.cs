using MailCheck.Common.Util;
using MailCheck.Insights.Contracts.Raw;
using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.DataSavers.Raw
{
    public interface INormalisedRecordFactory
    {
        NormalisedRecord Create(AggregateReportRecordEnriched source);
    }

    public class NormalisedRecordFactory : INormalisedRecordFactory
    {
        public NormalisedRecord Create(AggregateReportRecordEnriched source)
        {
            string domain = DomainNameUtils.ToCanonicalDomainName(source.HeaderFrom ?? source.DomainFrom);

            NormalisedRecord target = new NormalisedRecord
            {
                Adkim = source.Adkim?.ToString(),
                Aspf = source.Aspf?.ToString(),
                Arc = source.Arc ? source.Count : 0,
                BounceReflectorBlockListCount = source.BounceReflectorBlockListCount,
                Count = source.Count,
                Disposition = source.Disposition?.ToString(),
                Dkim = source.Dkim?.ToString(),
                DkimAuthResults = string.Join(',', source.DkimAuthResults),
                DkimFailCount = source.DkimFailCount,
                DkimPassCount = source.DkimPassCount,
                Domain = domain,
                DomainFrom = source.DomainFrom,
                EffectiveDate = source.EffectiveDate.Date,
                EndUserBlockListCount = source.EndUserBlockListCount,
                EndUserNetworkBlockListCount = source.EndUserNetworkBlockListCount,
                EnvelopeFrom = source.EnvelopeFrom,
                EnvelopeTo = source.EnvelopeTo,
                Fo = source.Fo,
                Forwarded = source.Forwarded ? source.Count : 0,
                HeaderFrom = source.HeaderFrom,
                HijackedNetworkBlockListCount = source.HijackedNetworkBlockListCount,
                HostAsDescription = source.HostAsDescription,
                HostAsNumber = source.HostAsNumber,
                HostCountry = source.HostCountry,
                HostName = source.HostName,
                HostOrgDomain = source.HostOrgDomain,
                HostProvider = source.HostProvider,
                HostSourceIp = source.HostSourceIp,
                LocalPolicy = source.LocalPolicy ? source.Count : 0,
                MailingList = source.MailingList ? source.Count : 0,
                MalwareBlockListCount = source.MalwareBlockListCount,
                OrganisationDomainFrom = source.OrganisationDomainFrom,
                OtherOverrideReason = source.OtherOverrideReason ? source.Count : 0,
                P = source.P.ToString(),
                Pct = source.Pct,
                ProxyBlockListCount = source.ProxyBlockListCount,
                RecordId = long.Parse(source.RecordId),
                ReporterOrgName = source.ReporterOrgName,
                ReportId = source.ReportId,
                ReverseDomain = DomainNameUtils.ReverseDomainName(domain),
                SampledOut = source.SampledOut ? source.Count : 0,
                Sp = source.Sp?.ToString(),
                SpamSourceBlockListCount = source.SpamSourceBlockListCount,
                Spf = source.Spf?.ToString(),
                SpfAuthResults = string.Join(',', source.SpfAuthResults),
                SpfFailCount = source.SpfFailCount,
                SpfPassCount = source.SpfPassCount,
                SuspiciousNetworkBlockListCount = source.SuspiciousNetworkBlockListCount,
                TrustedForwarder = source.TrustedForwarder ? source.Count : 0,
            };

            return target;
        }
    }
}