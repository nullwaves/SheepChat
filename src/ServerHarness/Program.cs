using SheepChat.Server;
using SheepChat.Server.Interfaces;
using System;

namespace ServerHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerManager server = ServerManager.Instance;
            server.SubscribeToSystemHost(new ConsoleUpdater());
            server.Start();

            while(true)
            {
                var s = Console.ReadLine();
                if (s == "quit")
                    server.Stop();
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
