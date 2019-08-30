using SheepChat.Server;
using System;

namespace ServerHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server;
            if (args.Length > 0 && int.TryParse(args[0], out int p))
            {
                server = new Server(p);
            }
            else
            {
                server = new Server();
            }
            server.Start();

            while(true)
            {
                var s = Console.ReadLine();
                if (s == "quit")
                    break;
            }
        }
    }
}
