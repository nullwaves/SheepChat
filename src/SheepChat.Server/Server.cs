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
using SheepChat.Server.Interfaces;

namespace SheepChat.Server
{
    public class Server : ISubSystem
    {
        /// <summary>
        /// Port on which the server should listen.
        /// </summary>
        public int Port { get; set; }

        #region Events
        /// <summary>
        /// A 'client connected' event raised by the server.
        /// </summary>
        public event EventHandler<ConnectionArgs> ClientConnect;

        /// <summary>
        /// A 'client disconnected' event raised by the server.
        /// </summary>
        public event EventHandler<ConnectionArgs> ClientDisconnected;

        /// <summary>
        /// A 'data received' event raised by the server.
        /// </summary>
        public event EventHandler<ConnectionArgs> DataReceived;

        /// <summary>
        /// A 'data sent' event raised by the server.
        /// </summary>
        public event EventHandler<ConnectionArgs> DataSent;
        #endregion

        /// <summary>
        /// Lock object for synchronization.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// List of current connections to this server.
        /// </summary>
        private readonly List<IConnection> connections = new List<IConnection>();

        /// <summary>
        /// The primary socket for incoming connections.
        /// </summary>
        private Socket socket;


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
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, Port));
        }

        public void Start()
        {
            // Start threads
            socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
            serverlog.Append("Listening for clients on port " + Port);
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = socket.EndAccept(ar);
                var connection = new Connection(clientSocket, this);
                connection.DataSent += HandleDataSent;
                connection.DataReceived += HandleDataReceived;
                connection.ClientDisconnected += HandleClientDisconnected;
                connection.BeginListen();

                lock(Lock)
                {
                    connections.Add(connection);
                }

                ClientConnect?.Invoke(this, new ConnectionArgs(connection));

                socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
                // Object is disposed, let the thread die.
            }
        }

        private void HandleClientDisconnected(object sender, ConnectionArgs e)
        {
            ClientDisconnected?.Invoke(sender, e);
        }

        private void HandleDataReceived(object sender, ConnectionArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        private void HandleDataSent(object sender, ConnectionArgs e)
        {
            DataSent?.Invoke(sender, e);
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
        private void acceptClients()
        {
            socket.Listen(4);
            while (true)
            {
                Socket s = socket.Accept();
                connections.Add(new Connection(s, this));
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

        public void SubscribeToSystem(ISubSystemHost sender)
        {
            throw new NotImplementedException();
        }

        public void InformSubscribedSystem(string msg)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }


}