using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Sheep.Logging;
using System;
using System.Threading.Tasks;

namespace Sheep.Telnet
{
    public class Server
    {
        internal int Port = 23;
        static Socket server;
        private static Task serverTask, consoleTask;

        internal static Logger serverlog = new Logger("server.log");
        
        public Server()
        {
            new Server(23);
        }

        public Server(int port)
        {
            Port = (port > 65535 || port < 1) ? 23 : port;

            // Show version info and state falsehoods
            Console.Title = "Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            serverlog.Append("Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            serverlog.Append("Initializing server on port " + Port);

            // Load usernames and passwords
            Dictionary<string, string> udata = LoadUserData();
            serverlog.Append(udata.Count() + " Users Loaded");
            // Save just incase?
            SaveUserData(udata);

            // Actual server initialization stuff
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, Port));
        }

        public Task Start()
        {
            // Start threads
            serverTask = Task.Run(acceptClients);
            serverlog.Append("Listening for clients on port " + Port);
            consoleTask = Task.Run(listenConsole);
            return serverTask;
        }

        // Spy on the console for admin commands
        private static void listenConsole()
        {
            Connection fake = new Connection(serverlog, LoadUserData());
            while (true)
            {
                string str = Console.ReadLine();
                fake.ProcessLine(str);
            }
        }

        // Load userdata from a flat file
        public static Dictionary<string, string> LoadUserData()
        {
            Dictionary<string, string> users = new Dictionary<string, string>();

            if (File.Exists("user.dat"))
            {
                string ins = Encoding.UTF8.GetString(File.ReadAllBytes("user.dat"));
                string[] pairs = ins.Split(';');
                foreach (string str in pairs)
                {
                    string[] str2 = str.Split(':');
                    if (str2[0] == "&") break;
                    users.Add(str2[0], str2[1]);
                }
            }
            else
            {
                SaveUserData(users);
            }

            return users;
        }

        // Save userdata to a flat file
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

        // Accept incoming connections loop
        private static void acceptClients()
        {
            server.Listen(100);
            while (true)
            {
                Socket conn = server.Accept();
                new Connection(conn);
            }
        }

        // Helper function to filter input from connections
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