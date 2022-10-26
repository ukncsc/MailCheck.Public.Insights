using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.SummaryGenerator.InsightGenerators;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.Config;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse.EvaluationRules;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains.EvaluationRules;
using Microsoft.Extensions.DependencyInjection;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyMixed;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyNone;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Configuration.EvaluationRules.PolicyQuarantineReject;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Factories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;

namespace MailCheck.Insights.SummaryGenerator.StartUp
{
    public class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IDatabase, DefaultDatabase<MySqlProvider>>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IHandle<InsightEvaluationPending>, InsightEvaluationPendingHandler>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<ISummaryGeneratorConfig, SummaryGeneratorConfig>()
                .AddTransient<IInsightGenerator, AbuseInsightGenerator>()
                .AddTransient<IInsightGenerator, ConfigurationInsightGenerator>()
                .AddTransient<IInsightGenerator, SubdomainInsightGenerator>()
                .AddTransient<IAbuseInsightGeneratorDao, AbuseInsightGeneratorDao>()
                .AddTransient<ISubdomainInsightGeneratorDao, SubdomainInsightGeneratorDao>()
                .AddTransient<ISubdomainInsightAdvisoryMessageGenerator, SubdomainInsightAdvisoryMessageGenerator>()
                .AddTransient<IEvaluator<AbuseEvaluationObject>, Evaluator<AbuseEvaluationObject>>()
                .AddTransient<IEvaluator<ConfigurationEvaluationObject>, Evaluator<ConfigurationEvaluationObject>>()
                .AddTransient<IEvaluator<AbuseEvaluationObject>, Evaluator<AbuseEvaluationObject>>()
                .AddTransient<IEvaluator<SubdomainsEvaluationObject>, Evaluator<SubdomainsEvaluationObject>>()
                .AddTransient<IRule<AbuseEvaluationObject>, AbuseRateWithAllPolicies>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyMixedInfo>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyMixedWarning>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyNoneInfo>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyNoneWarning>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyQuarantineRejectSuccess>()
                .AddTransient<IRule<ConfigurationEvaluationObject>, PolicyQuarantineRejectWarning>()
                .AddTransient<IRule<SubdomainsEvaluationObject>, DispositionNone>()
                .AddTransient<IRule<SubdomainsEvaluationObject>, DispositionQuarantineOrRejectLessThan5Percent>()
                .AddTransient<IRule<SubdomainsEvaluationObject>, DispositionQuarantineOrRejectMoreThan5Percent>()
                .AddTransient<IClock, Clock>()
                .AddSingleton<IProviderAliasProvider, ProviderAliasProvider>()
                .AddTransient<IConfigurationInsightGeneratorDao, ConfigurationInsightGeneratorDao>()
                .AddTransient<IConfigurationTableFactory, ConfigurationTableFactory>()
                .AddTransient<IExcludedTrafficFilter, ExcludedTrafficFilter>()
                .AddTransient<IConfigurationEvaluationObjectFactory, ConfigurationEvaluationObjectFactory>()
                .AddTransient<IFlaggedTrafficDataFactory, FlaggedTrafficDataFactory>()
                .AddTransient<IFilter, BlocklistFilter>()
                .AddTransient<IFilter, ReverseDnsFilter>()
                .AddTransient<IFilter, DmarcPassFilter>()
                .AddTransient<IFilter, DmarcOverrideFilter>();
        }
    }
}
