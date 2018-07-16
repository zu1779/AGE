namespace Zu1779.AGE.TestServerConsoleApp
{
    using System;
    using System.Reflection;

    using Zu1779.AGE.MainEngine;

    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program(args);
            program.Execute();
        }

        public Program(string[] args) { }
        private EngineManager engineManager;

        public void Execute()
        {
            using (engineManager = new EngineManager())
            {

                string input = null;
                while (input?.ToLower() != "exit")
                {
                    Console.WriteLine(Assembly.GetExecutingAssembly().GetName());
                    Console.Write("> ");
                    input = Console.ReadLine();
                    executeInput(input);
                }

            }
        }

        private void executeInput(string input)
        {
            if (input == "exit") Console.WriteLine("Exiting");
            else Console.WriteLine("Uknown command");
        }
    }
}
