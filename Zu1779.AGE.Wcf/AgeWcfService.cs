namespace Zu1779.AGE.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;

    using Zu1779.AGE.MainEngine;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AgeWcfService : IAgeWcfService
    {
        public AgeWcfService(EngineManager engineManager)
        {
            this.engineManager = engineManager ?? throw new ArgumentNullException(nameof(engineManager));
        }
        private readonly EngineManager engineManager;

        public string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().ToString();
        }

        public List<string> ExecuteCommand(string inputCommand)
        {
            List<string> returnValue = new List<string>();
            var arrInput = GetInputArray(inputCommand);
            if (arrInput.Any())
            {
                if (arrInput[0] == "exit") returnValue.Add("Exiting");
                else if (arrInput[0] == "test") Test();
                else if (arrInput.Skip(1).Any())
                {
                    if (arrInput[0] == "env") Env(arrInput);
                    else if (arrInput[0] == "card") Card(arrInput);
                    else returnValue.Add("Uknown command");
                }
                else returnValue.Add($"Not enough parameters for '{arrInput[0]}' command");
            }
            return returnValue;
        }

        private List<string> GetInputArray(string input)
        {
            Regex regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)");
            var result = regex.Matches(input).Cast<Match>().Where(c => c.Groups["token"].Success).Select(c => c.Groups["token"].Value).ToList();
            return result;
        }

        private void Test()
        {
            string environmentCode = "test";
            string environmentPath = @"H:\Progetti\AGE\Zu1779.AGE.Environment.TestEnvironment\bin\Debug";
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

        private void Card(List<string> arrInput)
        {
            string environmentCode = "CardGame";
            if (arrInput[1] == "check")
            {
                engineManager.CheckStatusEnvironment(environmentCode);
            }
            else if (arrInput[1] == "setup")
            {
                string environmentPath = @"H:\Progetti\AGE\Zu1779.AGE.Env.CardGameEnv\bin\Debug";
                engineManager.AddEnvironment(environmentCode, environmentPath);

                string agentPath = @"H:\Progetti\AGE\Zu1779.AGE.Agent.CardGameAgent\bin\Debug";
                engineManager.AddAgent(environmentCode, "agent001", agentPath);
                engineManager.AddAgent(environmentCode, "agent002", agentPath);
                engineManager.AddAgent(environmentCode, "agent003", agentPath);
                engineManager.AddAgent(environmentCode, "agent004", agentPath);

                engineManager.SetUpEnvironment(environmentCode);
            }
            else if (arrInput[1] == "start")
            {
                engineManager.StartEnvironment(environmentCode);
            }
            else if (arrInput[1] == "pause")
            {
                engineManager.PauseEnvironment(environmentCode);
            }
            else if (arrInput[1] == "continue")
            {
                engineManager.ContinueEnvironment(environmentCode);
            }
            else if (arrInput[1] == "stop")
            {
                engineManager.StopEnvironment(environmentCode);
            }
            else if (arrInput[1] == "teardown")
            {
                engineManager.TearDownEnvironment(environmentCode);
            }
            else Console.WriteLine($"Unknown command: {string.Join(" ", arrInput)}");
        }

        private void Env(List<string> arrInput)
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
                if (listEnv.Any()) foreach (var item in listEnv) Console.WriteLine(item.Code);
                if (listEnv.Any()) Console.WriteLine(string.Join("\r\n", listEnv.Select(c => $"[{c.InstanceTime}] {c.Code}")));
                else Console.WriteLine("No environment yet");
            }
        }
    }
}
