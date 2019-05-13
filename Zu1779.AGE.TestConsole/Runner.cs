namespace Zu1779.AGE.TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.Logging;

    using Zu1779.AGE.WcfClient;

    public class Runner
    {
        public Runner(string[] args, IAgeWcfService ageWcfService, ILogger<Runner> logger)
        {
            this.args = new Queue<string>(args);
            client = ageWcfService;
            this.logger = logger;
        }
        private readonly Queue<string> args;
        private readonly IAgeWcfService client;
        private readonly ILogger<Runner> logger;

        public void Execute()
        {
            logger.LogTrace("TRACE");
            logger.LogDebug("DEBUG");
            logger.LogInformation("INFO");
            logger.LogWarning("WARN");
            logger.LogError("ERROR");
            logger.LogCritical("CRITICAL");

            logger.LogInformation($"Received {args.Count}\r\nArguments: {string.Join("\r\n", args.Select((c, i) => $"[{i}] {c}"))}");

            string input = null;
            while (input != "exit")
            {
                Console.WriteLine(Assembly.GetExecutingAssembly().GetName());
                Console.Write("> ");
                input = Console.ReadLine().ToLower();
                try
                {
                    var response = client.ExecuteCommandAsync(new ExecuteCommandRequest { inputCommand = input });
                    response.Result.ExecuteCommandResult.ForEach(c => Console.WriteLine(c));
                }
                catch (Exception ex)
                {
                    logger.LogError($"Command {input} failed: {ex.Message}");
                }
            }
        }
    }
}
