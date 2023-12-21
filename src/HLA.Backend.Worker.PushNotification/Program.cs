using Confluent.Kafka;
using HLA.Backend.Core.DataAccess;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.Infra.ApacheKafka;
using HLA.Backend.Core.Infra.FCM;
using HLA.Backend.Core.Infra.FCM.Implementations;
using HLA.Backend.Worker.PushNotification;
using HLA.Backend.Worker.PushNotification.Services;
using Serilog;


IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService( options =>
    {
        options.ServiceName = "HLA-PushNotification";
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.AddHostedService<Worker>();
        services.AddLogging(builder =>
        {
            var logger = new LoggerConfiguration()
                          .ReadFrom.Configuration(configuration)
                          .CreateLogger();
            builder.AddSerilog(logger);
        });

        var consumerConfig = new ConsumerConfig();
        configuration.Bind("KafkaConsumerConfig", consumerConfig);
        Consumer<string,string> consumerWrapper = new Consumer<string, string>(consumerConfig);
        services.AddSingleton<Consumer<string, string>>(consumerWrapper);

        //Worker Services
        services.AddSingleton<IKafkaPushNotificationServices, KafkaPushNotificationServices>();

        //Database Service
        services.AddSingleton<DbContext>();
        services.AddSingleton<INotificationRepo, NotificationRepo>();
        services.AddSingleton<IRegisterRepo, RegisterRepo>();

        //FCM Services
        services.AddSingleton<IFcmFactory, FcmFactory>();
        services.AddSingleton<FcmHla360Services>();
        services.AddSingleton<FcmEcrmServices>();
        services.AddSingleton<FcmEcrmBancaServices>();
    })
    .Build();

await host.RunAsync();
