using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.Insights.Api
{
    public interface IInsightsApiConfig
    {
        string MicroserviceOutputSnsTopicArn { get; }
    }

    public class InsightsApiConfig : IInsightsApiConfig
    {
        public InsightsApiConfig(IEnvironmentVariables environmentVariables)
        {
            MicroserviceOutputSnsTopicArn = environmentVariables.Get("MicroserviceOutputSnsTopicArn");
        }
        public string MicroserviceOutputSnsTopicArn { get; }
    }
}