using System;
using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.DataSaver.Contract
{
    public class AggregateReportRecordEnriched : Message
    {
        public AggregateReportRecordEnriched(string id) : base(id)
        {
        }

        public Alignment? Adkim { get; set; }
        public Alignment? Aspf { get; set; }
        public bool Arc { get; set; }
        public int BounceReflectorBlockListCount { get; set; }
        public int Count { get; set; }
        public Policy? Disposition { get; set; }
        public DmarcResult? Dkim { get; set; }
        public List<string> DkimAuthResults { get; set; }
        public int DkimFailCount { get; set; }
        public int DkimPassCount { get; set; }
        public string DomainFrom { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int EndUserBlockListCount { get; set; }
        public int EndUserNetworkBlockListCount { get; set; }
        public string EnvelopeFrom { get; set; }
        public string EnvelopeTo { get; set; }
        public string Fo { get; set; }
        public bool Forwarded { get; set; }
        public string HeaderFrom { get; set; }
        public int HijackedNetworkBlockListCount { get; set; }
        public string HostAsDescription { get; set; }
        public int HostAsNumber { get; set; }
        public string HostCountry { get; set; }
        public string HostName { get; set; }
        public string HostOrgDomain { get; set; }
        public string HostProvider { get; set; }
        public string HostSourceIp { get; set; }
        public bool LocalPolicy { get; set; }
        public bool MailingList { get; set; }
        public int MalwareBlockListCount { get; set; }
        public string OrganisationDomainFrom { get; set; }
        public bool OtherOverrideReason { get; set; }
        public Policy P { get; set; }
        public int? Pct { get; set; }
        public int ProxyBlockListCount { get; set; }
        public string RecordId { get; set; }
        public string ReporterOrgName { get; set; }
        public string ReportId { get; set; }
        public bool SampledOut { get; set; }
        public Policy? Sp { get; set; }
        public int SpamSourceBlockListCount { get; set; }
        public int SuspiciousNetworkBlockListCount { get; set; }
        public DmarcResult? Spf { get; set; }
        public List<string> SpfAuthResults { get; set; }
        public int SpfFailCount { get; set; }
        public int SpfPassCount { get; set; }
        public bool TrustedForwarder { get; set; }
    }
}