namespace Zu1779.AGE.TestServerConsoleApp
{
    using System;
    using System.Reflection;
    using System.ServiceModel;

    using Common.Logging;

    using Zu1779.AGE.MainEngine;
    using Zu1779.AGE.Wcf;

    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            //log.Trace(c => c("TRACE"));
            //log.Debug(c => c("DEBUG"));
            //log.Info(c => c("INFO"));
            //log.Warn(c => c("WARN"));
            //log.Error(c => c("ERROR"));
            //log.Fatal(c => c("FATAL"));
            //Console.ReadKey();
            //return;

            var program = new Program(args);
            program.Execute();
        }

        public Program(string[] args) { }
        private EngineManager engineManager;
        private ServiceHost serviceHost;

        public void Execute()
        {
            ILog logEngineManager = LogManager.GetLogger<EngineManager>();
            using (engineManager = new EngineManager(logEngineManager))
            {
                startWcfInterface();

                string input = null;
                while (input?.ToLower() != "exit")
                {
                    Console.WriteLine(Assembly.GetExecutingAssembly().GetName());
                    Console.Write("> ");
                    input = Console.ReadLine().ToLower();
                    try
                    {
                        executeInput(input);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Command {input} failed: {ex.Message}");
                    }
                }

                stopWcfInterface();
            }
        }

        private void executeInput(string input)
        {
            if (input == "exit") Console.WriteLine("Exiting");
            else if (input.StartsWith("addenv")) engineManager.CreateEnvironment();
            else Console.WriteLine("Uknown command");
        }

        private void startWcfInterface()
        {
            log.Info($"Starting WCF Interface");
            var ageWcfService = new AgeWcfService(engineManager);
            serviceHost = new ServiceHost(ageWcfService);
            serviceHost.Open();
            log.Info($"Started WCF Interface");
        }

        private void stopWcfInterface()
        {
            log.Info($"Stopping WCF Interface");
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
            log.Info($"Stopped WCF Interface");
        }
    }
}
