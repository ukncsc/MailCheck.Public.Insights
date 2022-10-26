using System;
using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.Insights.SummaryGenerator.Config
{
    public interface ISummaryGeneratorConfig
    {
        string SnsTopicArn { get; }
        TimeSpan SummaryPeriod { get; set; }
        TimeSpan WaitForDataPeriod { get; set; }
        string MailCheckUrlPrefix { get; set; }
    }

    public class SummaryGeneratorConfig : ISummaryGeneratorConfig
    {
        public SummaryGeneratorConfig(IEnvironmentVariables environmentVariables)
        {
            SnsTopicArn = environmentVariables.Get("SnsTopicArn");
            SummaryPeriod = TimeSpan.FromDays(environmentVariables.GetAsInt("SummaryPeriod"));
            WaitForDataPeriod = TimeSpan.FromDays(environmentVariables.GetAsInt("WaitForDataPeriod"));
            MailCheckUrlPrefix = environmentVariables.Get("MailCheckUrlPrefix");
        }

        public string SnsTopicArn { get; }
        public TimeSpan SummaryPeriod { get; set; }
        public TimeSpan WaitForDataPeriod { get; set; }
        public string MailCheckUrlPrefix { get; set; }
    }
}
