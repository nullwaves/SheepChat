using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
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

        /// <summary>
        /// Sub system host.
        /// </summary>
        private ISubSystemHost subSystemHost;

        public Server() : this(23) { }

        public Server(int port)
        {
            Port = (port > 65535 || port < 1) ? 23 : port;

            // Load usernames and passwords
            Dictionary<string, string> udata = LoadUserData();
            // Save just incase?
            SaveUserData(udata);

        }

        /// <summary>
        /// Initialize server and start listening for connections.
        /// </summary>
        public void Start()
        {
            // Actual server initialization stuff
            socket = new Socket(AddressFamily.InterNetwork,
                                SocketType.Stream,
                                ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            socket.Listen(4);

            // Start thread
            socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
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

                lock (Lock)
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
            string strdata = "";
            foreach (KeyValuePair<string, string> str in udata)
            {
                strdata += str.Key + ":" + str.Value + ";";
            }
            strdata += "&";

            byte[] outs = Encoding.UTF8.GetBytes(strdata);

            File.WriteAllBytes("user.dat", outs);
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

        /// <summary>
        /// Subscribe to receive system updates from this system.
        /// </summary>
        /// <param name="sender">System to subscribe to.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            subSystemHost = sender;
        }

        /// <summary>
        /// Inform subscribed system of an update.
        /// </summary>
        /// <param name="message">Message to be sent to subscribed systems.</param>
        public void InformSubscribedSystem(string message)
        {
            subSystemHost.UpdateSubSystemHost(this, message);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            CloseAllSockets();
        }

        /// <summary>
        /// Close server socket and all connections.
        /// </summary>
        private void CloseAllSockets()
        {
            socket?.Close();

            var connectionsCopy = new List<IConnection>(connections);
            foreach(var connection in connectionsCopy)
            {
                connection.Send("Server shutting down. Disconnecting...");
                connection.Disconnect();
            }
        }
    }


}