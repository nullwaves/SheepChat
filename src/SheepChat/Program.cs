using SheepChat.Server;
using SheepChat.Server.Interfaces;
using System;
using System.Linq;

namespace ServerHarness
{
    internal class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter

        private static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var stopWords = CommandManager.QuitKeywords.ToList();

            Application app = new Application();
            app.Start();

            while (true)
            {
                var s = Console.ReadLine().Trim();
                if (stopWords.Contains(s))
                {
                    app.Stop();
                    break;
                }
            }
        }
    }

    public class ConsoleUpdater : ISystemHost
    {
        public void UpdateSystemHost(ISystem sender, string msg)
        {
            Console.WriteLine("<{0:s}> {1}", DateTime.Now, msg);
        }
    }
}