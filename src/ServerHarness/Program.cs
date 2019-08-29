﻿using SheepChat.Server;
using System.Threading.Tasks;

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
        }
    }
}
