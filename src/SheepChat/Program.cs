using SheepChat.Server;
using SheepChat.Server.Interfaces;
using System;

namespace ServerHarness
{
    class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Application app = new Application();
            app.Start();

            while(true)
            {
                var s = Console.ReadLine();
                if (s == "quit")
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
