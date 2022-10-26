using System;

namespace MailCheck.Insights.Contracts.Raw
{
    public class NormalisedRecord
    {
        public string Adkim { get; set; }
        public string Aspf { get; set; }
        public int Arc { get; set; }
        public int BounceReflectorBlockListCount { get; set; }
        public int Count { get; set; }
        public string Disposition { get; set; }
        public string Dkim { get; set; }
        public string DkimAuthResults { get; set; }
        public int DkimFailCount { get; set; }
        public int DkimPassCount { get; set; }
        public string Domain { get; set; }
        public string DomainFrom { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int EndUserBlockListCount { get; set; }
        public int EndUserNetworkBlockListCount { get; set; }
        public string EnvelopeFrom { get; set; }
        public string EnvelopeTo { get; set; }
        public string Fo { get; set; }
        public int Forwarded { get; set; }
        public string HeaderFrom { get; set; }
        public int HijackedNetworkBlockListCount { get; set; }
        public string HostAsDescription { get; set; }
        public int HostAsNumber { get; set; }
        public string HostCountry { get; set; }
        public string HostName { get; set; }
        public string HostOrgDomain { get; set; }
        public string HostProvider { get; set; }
        public string HostSourceIp { get; set; }
        public int LocalPolicy { get; set; }
        public int MailingList { get; set; }
        public int MalwareBlockListCount { get; set; }
        public string OrganisationDomainFrom { get; set; }
        public int OtherOverrideReason { get; set; }
        public string P { get; set; }
        public int? Pct { get; set; }
        public int ProxyBlockListCount { get; set; }
        public long RecordId { get; set; }
        public string ReporterOrgName { get; set; }
        public string ReportId { get; set; }
        public string ReverseDomain { get; set; }
        public int SampledOut { get; set; }
        public string Sp { get; set; }
        public int SpamSourceBlockListCount { get; set; }
        public int SuspiciousNetworkBlockListCount { get; set; }
        public string Spf { get; set; }
        public string SpfAuthResults { get; set; }
        public int SpfFailCount { get; set; }
        public int SpfPassCount { get; set; }
        public int TrustedForwarder { get; set; }
    }
}