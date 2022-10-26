using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers;
using MailCheck.Insights.DataSaver.DataSavers.Abuse;
using MailCheck.Insights.DataSaver.DataSavers.Configuration;
using MailCheck.Insights.DataSaver.DataSavers.Subdomains;
using MailCheck.Insights.DataSaver.DataSavers.Raw;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.Insights.DataSaver.StartUp
{
    public class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<IHandle<AggregateReportRecordEnriched>, AggregateReportRecordEnrichedHandler>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IRawDataSaver, RawDataSaver>()
                .AddTransient<IDataSaver, ConfigurationDataSaver>()
                .AddTransient<IDataSaver, SubdomainDataSaver>()
                .AddTransient<IDataSaver, AbuseDataSaver>()
                .AddSingleton<IDatabase, DefaultDatabase<MySqlProvider>>()
                .AddSingleton<INormalisedRecordFactory, NormalisedRecordFactory>()
                .AddSingleton<ISubdomainDataFactory, SubdomainDataFactory>()
                .AddSingleton<IConfigurationDataFactory, ConfigurationDataFactory>()
                .AddSingleton<IAbuseDataSaverFactory, AbuseDataFactory>();
        }
    }
}
