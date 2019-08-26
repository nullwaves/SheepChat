using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Sheep.Logging;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace SheepsTelnetChat
{
    class Server
    {
        static int PortNumber = 23;
        const int BacklogSize = 20;
        static Socket server;
        private static Task serverTask, consoleTask;

        public static Logger serverlog = new Logger("server.log");
        
        static void Main(string[] args)
        {
            // Handle command line arguments from launch
            // TODO: Remove Main completely from the Server framework
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
                Console.WriteLine(e.Message + " " + e.StackTrace);
            }

            // Show version info and state falsehoods
            Console.Title = "Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            serverlog.Append("Sheep's Telnet Chat Server v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            serverlog.Append("Server initiated");

            // Load usernames and passwords
            Dictionary<string,string> udata = LoadUserData();
            serverlog.Append(udata.Count() + " Users Loaded");
            // Save just incase?
            SaveUserData(udata);

            // Actual server initialization stuff
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, PortNumber));
            server.Listen(BacklogSize);
            serverlog.Append("Listening for clients on port " + PortNumber);

            // Start threads
            serverTask = Task.Run(acceptClients);
            consoleTask = Task.Run(listenConsole);
            serverTask.Wait();
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