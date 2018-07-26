namespace Zu1779.AGE.TestServerConsoleApp
{
    using System;
    using System.Reflection;
    using System.ServiceModel;

    using log4net;

    using Zu1779.AGE.MainEngine;
    using Zu1779.AGE.Wcf;

    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(Program));

        static void Main(string[] args)
        {
            var program = new Program(args);
            program.Execute();
        }

        public Program(string[] args) { }
        private EngineManager engineManager;
        private ServiceHost serviceHost;

        public void Execute()
        {
            using (engineManager = new EngineManager())
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
                        Console.WriteLine($"EXCEPTION: {ex.Message}");
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
