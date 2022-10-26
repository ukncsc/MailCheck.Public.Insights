using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.Insights.Entity.Config
{
    public interface IInsightEntityConfig
    {
        string SnsTopicArn { get; }
        string WebUrl { get; }
    }

    public class InsightEntityConfig : IInsightEntityConfig
    {
        public InsightEntityConfig(IEnvironmentVariables environmentVariables)
        {
            SnsTopicArn = environmentVariables.Get("SnsTopicArn");
            WebUrl = environmentVariables.Get("WebUrl");
        }

        public string SnsTopicArn { get; }
        public string WebUrl { get; }
    }
}
