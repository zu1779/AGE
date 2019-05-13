namespace Zu1779.AGE.TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Extensions.Logging;

    using Zu1779.AGE.WcfClient;

    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                    //.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                IServiceProvider servicesProvider = BuildDI(args, config);

                using ((IDisposable)servicesProvider)
                {
                    var runner = servicesProvider.GetRequiredService<Runner>();
                    runner.Execute();

                    Console.WriteLine("Press ANY key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }

            Environment.Exit(0);
        }
        static IServiceProvider BuildDI(string[] args, IConfiguration config)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<Runner>()
                .AddSingleton(args)
                .AddTransient<IAgeWcfService, AgeWcfServiceClient>()
                .AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                })
                .BuildServiceProvider();
            return serviceProvider;
        }
    }
}
