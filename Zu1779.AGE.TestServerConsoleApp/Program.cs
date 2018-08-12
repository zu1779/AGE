namespace Zu1779.AGE.TestServerConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;
    using System.Threading;

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

        public Program(string[] args)
        {
            this.args = new Queue<string>(args);
        }
        private readonly Queue<string> args;
        private EngineManager engineManager;
        private ServiceHost serviceHost;

        public void Execute()
        {
            ILog logEngineManager = LogManager.GetLogger<EngineManager>();
            using (engineManager = new EngineManager(logEngineManager))
            {
                startWcfInterface();

                string input = null;
                while (input != "exit")
                {
                    Console.WriteLine(Assembly.GetExecutingAssembly().GetName());
                    Console.Write("> ");
                    if (args.Count > 0)
                    {
                        input = args.Dequeue();
                        Console.WriteLine(input);
                    }
                    else input = Console.ReadLine().ToLower();
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
            else if (arrInput[0] == "test") test();
            else if (arrInput[0] == "card") card();
            else Console.WriteLine("Uknown command");
        }
        private List<string> getInputArray(string input)
        {
            Regex regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)");
            var result = regex.Matches(input).Cast<Match>().Where(c => c.Groups["token"].Success).Select(c => c.Groups["token"].Value).ToList();
            return result;
        }
        private void test()
        {
            string environmentCode = "test";
            string environmentPath = @"C:\Progetti\A.G.E\Zu1779.AGE\Zu1779.AGE.Environment.TestEnvironment\bin\Debug";
            engineManager.AddEnvironment(environmentCode, environmentPath);
            engineManager.AddAgent(environmentCode, "agent001", @"C:\Progetti\A.G.E\Zu1779.AGE\Zu1779.AGE.Agent.TestAgent\bin\Debug");
            engineManager.CheckStatusEnvironment(environmentCode);
            engineManager.SetUpEnvironment(environmentCode);
            engineManager.StartEnvironment(environmentCode);
            engineManager.PauseEnvironment(environmentCode);
            engineManager.ContinueEnvironment(environmentCode);
            engineManager.CommandEnvironment(environmentCode, 17);
            engineManager.StopEnvironment(environmentCode);
            engineManager.TearDownEnvironment(environmentCode);
        }
        private void card()
        {
            string environmentCode = "CardGame";
            string environmentPath = @"C:\Progetti\A.G.E\Zu1779.AGE\Zu1779.AGE.Env.CardGameEnv\bin\Debug";
            engineManager.AddEnvironment(environmentCode, environmentPath);
            engineManager.CheckStatusEnvironment(environmentCode);
            engineManager.SetUpEnvironment(environmentCode);
            engineManager.StartEnvironment(environmentCode);
            engineManager.PauseEnvironment(environmentCode);
            engineManager.ContinueEnvironment(environmentCode);
            engineManager.StopEnvironment(environmentCode);
            engineManager.TearDownEnvironment(environmentCode);
        }
        private void env(List<string> arrInput)
        {
            if (arrInput.Count < 2) throw new ApplicationException("Too few parameters. Usage: env [add|list] [<environment name>]");
            if (arrInput[1] == "add")
            {
                if (arrInput.Count < 3) throw new ApplicationException("Too few parameters. Usage: env add [<environment name>]");
                string environmentCode = arrInput[2];
                string environmentPath = @"C:\Progetti\A.G.E\Zu1779.AGE\Zu1779.AGE.Environment.TestEnvironment\bin\Debug";
                engineManager.AddEnvironment(environmentCode, environmentPath);
            }
            else if (arrInput[1] == "list")
            {
                var listEnv = engineManager.GetEnvironments().OrderByDescending(c => c.InstanceTime);
                //if (listEnv.Any()) foreach (var item in listEnv) Console.WriteLine(item.Code);
                if (listEnv.Any()) Console.WriteLine(string.Join("\r\n", listEnv.Select(c => $"[{c.InstanceTime}] {c.Code}")));
                else Console.WriteLine("No environment yet");
            }
        }

        #region WCF
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
        #endregion
    }
}
