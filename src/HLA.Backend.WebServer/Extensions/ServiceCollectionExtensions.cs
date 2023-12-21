using Serilog;
using Confluent.Kafka;
using HLA.Backend.WebServer.Filter.Security;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Application.Implementations;
using HLA.Backend.WebServer.Application.Workers;
using HLA.Backend.WebServer.Application.Workers.Implementations;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.DataAccess;
using HLA.Backend.Core.Infra.ApacheKafka;
using HLA.Backend.Core.Infra.FCM.Implementations;
using HLA.Backend.Core.Infra.FCM;
using HLA.Backend.Core.Domain.Auth;
using HLA.Backend.WebServer.Application;

namespace HLA.Backend.WebServer.Extensions
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration config)
        {
            AddSecurity(services, config);
            AddSeriLog(builder, config);
            AddDbContext(services, config);
            AddRepos(services);
            AddServices(services);
            AddKafkaServices(services, config);
            AddWorkers(services);
            AddFcmServices(services);
            AddAutoMapper(services);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddSecurity(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ApiKeyAuthorizationFilter>();
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
            var _authConfig = new AuthConfig();
            config.Bind("Auth", _authConfig.Auth);
            services.AddSingleton<AuthConfig>(_authConfig);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddSeriLog(this WebApplicationBuilder builder, IConfiguration config)
        {
            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(config)
              .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddDbContext(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<DbContext>();
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddRepos(IServiceCollection services)
        {
            services.AddSingleton<INotificationRepo, NotificationRepo>();
            services.AddSingleton<IRegisterRepo, RegisterRepo>();
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IInAppPushNotificationServices, InAppPushNotificationServices>();
            services.AddTransient<INotificationServices, NotificationServices>();
            services.AddTransient<IRegisterServices, RegisterServices>();
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddKafkaServices(IServiceCollection services, IConfiguration config)
        {
            var consumerConfig = new ConsumerConfig();
            var publisherConfig = new ProducerConfig();
            config.Bind("KafkaConfig", consumerConfig);
            config.Bind("KafkaConfig", publisherConfig);
            Consumer<string, string> consumerWrapper = new Consumer<string, string>(consumerConfig);
            Publisher publisherWrapper = new Publisher(publisherConfig);
            services.AddSingleton<Consumer<string, string>>(consumerWrapper);
            services.AddSingleton<Publisher>(publisherWrapper);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddWorkers(IServiceCollection services)
        {
            //Worker Services
            services.AddHostedService<PingWorker>();

            #region InApp Push Notification
            services.AddHostedService<PushNotificationWorker>();
            services.AddSingleton<ICommonPushNotification, CommonPushNotification>();
            services.AddSingleton<IKafkaPushNotificationServices, KafkaPushNotificationServices>();
            services.AddSingleton<IIntervalRunPushNotificationServices, IntervalRunPushNotificationServices>();
            services.AddSingleton<IBulkPushNotificationServices, BulkPushNotificationServices>();
            #endregion
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddFcmServices(IServiceCollection services)
        {
            services.AddSingleton<IFcmFactory, FcmFactory>();
            services.AddSingleton<FcmHla360Services>();
            services.AddSingleton<FcmEcrmServices>();
            services.AddSingleton<FcmEcrmBancaServices>();
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
        }
    }
}
