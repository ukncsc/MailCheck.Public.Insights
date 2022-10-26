using System;

namespace MailCheck.Insights.Contracts.Configuration
{
    public class ConfigurationData
    {
        public string Domain { get; set; }
        public string HostName { get; set; }
        public string Provider { get; set; }
        public string Policy { get; set; }
        public int Traffic { get; set; }
        public int DmarcFail { get; set; }
        public int SpfAuthFail { get; set; }
        public int SpfMisaligned { get; set; }
        public int DkimAuthFail { get; set; }
        public int DkimMisaligned { get; set; }
    }
}