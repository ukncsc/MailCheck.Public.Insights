using System;

namespace MailCheck.Insights.DataSaver.DataSavers.Subdomains
{
    public class SubdomainData
    {
        public string RecordId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Domain { get; set; }
        public string ReverseDomain { get; set; }
        public int AllTrafficCount { get; set; }
        public int DkimOrSpfFailedNoneCount { get; set; }
        public int DkimOrSpfFailedQuarantineOrRejectCount { get; set; }
        public int DkimAndSpfFailedNoneCount { get; set; }
        public int DkimAndSpfFailedQuarantineOrRejectCount { get; set; }
        public int BlockListOrFailedReverseDnsCount { get; set; }
    }
}