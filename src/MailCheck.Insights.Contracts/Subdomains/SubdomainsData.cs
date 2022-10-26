using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.Insights.Contracts.Subdomains
{
    public class SubdomainsData
    {
        public string Domain { get; set; }
        public string Policy { get; set; }
        public int AllTrafficCount { get; set; }
        public int DmarcFailCount { get; set; }
    }
}
