namespace Zu1779.AGE.TestServerConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;

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
            var arrInput = getInputArray(input);
            if (arrInput[0] == "exit") Console.WriteLine("Exiting");
            else if (arrInput[0] == "env") env(arrInput);
            else Console.WriteLine("Uknown command");
        }
        private List<string> getInputArray(string input)
        {
            Regex regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)");
            var result = regex.Matches(input).Cast<Match>().Where(c => c.Groups["token"].Success).Select(c => c.Groups["token"].Value).ToList();
            return result;
        }
        private void env(List<string> arrInput)
        {
            if (arrInput.Count < 2) throw new ApplicationException("Too few parameters. Usage: env [add|list] [<environment name>]");
            if (arrInput[1] == "add")
            {
                string environmentCode = "testenv";
                string environmentPath = @"C:\Progetti\A.G.E\Zu1779.AGE\Zu1779.AGE.Environment.TestEnvironment\bin\Debug";
                engineManager.CreateEnvironment(environmentCode, environmentPath);
            }
            else if (arrInput[1] == "list")
            {
                var listEnv = engineManager.ListEnvironment();
                if (listEnv.Any()) foreach (var item in listEnv) Console.WriteLine(item.Code);
                else Console.WriteLine("No environment yet");
            }
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
