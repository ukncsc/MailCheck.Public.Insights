using System.Collections.Generic;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Processors.Notifiers;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using MailCheck.EmailSecurity.Entity.Entity.Notifiers;
using MailCheck.Insights.Entity.Config;
using MailCheck.Insights.Entity.Dao;
using MailCheck.Insights.Entity.Notifiers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using FindingsChangedNotifier = MailCheck.Common.Processors.Notifiers.FindingsChangedNotifier;
using LocalFindingsChangedNotifier = MailCheck.Insights.Entity.Notifiers.FindingsChangedNotifier;
using MessageEqualityComparer = MailCheck.EmailSecurity.Entity.Entity.Notifiers.MessageEqualityComparer;

namespace MailCheck.Insights.Entity.StartUp
{
    public class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings serializerSetting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };

                serializerSetting.Converters.Add(new StringEnumConverter());

                return serializerSetting;
            };

            services
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddSingleton<IClock, Clock>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IInsightEntityDao, InsightEntityDao>()
                .AddTransient<IInsightEntityConfig, InsightEntityConfig>()
                .AddTransient<IInsightEntityScheduler, InsightEntityScheduler>()
                .AddTransient<IChangeNotifier, LocalFindingsChangedNotifier>()
                .AddTransient<IChangeNotifiersComposite, ChangeNotifiersComposite>()
                .AddTransient<IFindingsChangedNotifier, FindingsChangedNotifier>()
                .AddTransient<InsightEntityHandler>()
                .AddTransient<IChangeNotifier, AdvisoryChangedNotifier>()
                .AddTransient<IEqualityComparer<AdvisoryMessage>, MessageEqualityComparer>();
        }
    }
}
