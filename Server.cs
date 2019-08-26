using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Common;
using Sheep.Logging;
using System.Threading;
using System;

namespace SheepsTelnetChat
{
    class Server
    {
        static int PortNumber = 23;
        const int BacklogSize = 20;
        static Socket server;
        static Thread serverThread,consoleThread;

        public static Logger serverlog = new Logger("server.log");
        
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0] != String.Empty)
                    {
                        PortNumber = Convert.ToInt32(args[0]);
                        if (PortNumber > 65535 || PortNumber < 1)
                        {
                            PortNumber = 23;
                            throw new Exception("Port must be between 0 and 65535; Using default port");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Title = "Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            serverlog.Append("Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            serverlog.Append("Server initiated");

            Dictionary<string,string> udata = LoadUserData();

            Connection fakeconn = new Connection(serverlog, udata);
            serverlog.Append(udata.Count() + " Users Loaded");
            SaveUserData(udata);
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, PortNumber));
            server.Listen(BacklogSize);
            
            serverlog.Append("Listening for clients on port " + PortNumber);
            
            serverThread = new Thread(acceptClients());
            serverThread.Start();
            consoleThread = new Thread(listenConsole(fakeconn));
            consoleThread.Start();
        }

        private static ThreadStart listenConsole(Connection c)
        {
            while (true)
            {
                string str = Console.ReadLine();
                c.ProcessLine(str);
            }
        }

        public static Dictionary<string, string> LoadUserData()
        {
            Dictionary<string, string> users = new Dictionary<string, string>();

            string ins = Encoding.UTF8.GetString(File.ReadAllBytes("user.dat"));
            string[] pairs = ins.Split(';');
            foreach (string str in pairs)
            {
                string[] str2 = str.Split(':');
                if(str2[0] == "&") break;
                users.Add(str2[0], str2[1]);
            }

            return users;
        }

        public static void SaveUserData(Dictionary<string, string> udata)
        {
            serverlog.Append("Saved User Data");

            string strdata = "";
            foreach (KeyValuePair<string, string> str in udata)
            {
                strdata += str.Key + ":" + str.Value + ";";
            }
            strdata += "&";

            byte[] outs = Encoding.UTF8.GetBytes(strdata);

            File.WriteAllBytes("user.dat", outs);
        }

        private static ThreadStart acceptClients()
        {
            while (true)
            {
                Socket conn = server.Accept();
                new Connection(conn);
            }
        }

        public static string ReplaceBackspace(string hasBackspace)
        {
            if (string.IsNullOrEmpty(hasBackspace))
                return hasBackspace;

            StringBuilder result = new StringBuilder(hasBackspace.Length);
            foreach (char c in hasBackspace)
            {
                if (c == '\b')
                {
                    if (result.Length > 0)
                        result.Length--;
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
    }


}