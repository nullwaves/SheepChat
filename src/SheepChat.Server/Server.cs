using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SheepChat.Server
{
    /// <summary>
    /// Server Sub-System that controls and handles the core socket functions of the server.
    /// </summary>
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

        #endregion Events

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

        /// <summary>
        /// Default constructor for a server with an unspecified port.
        /// </summary>
        public Server() : this(23) { }

        /// <summary>
        /// Constructor for a server with a specified port. Defaults to 23 if port number is out of range.
        /// </summary>
        /// <param name="port">Port number to open socket on</param>
        public Server(int port)
        {
            Port = (port > 65535 || port < 1) ? 23 : port;
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
            lock (Lock)
            {
                connections.Remove(e.Connection);
            }
            ClientDisconnected?.Invoke(this, e);
        }

        private void HandleDataReceived(object sender, ConnectionArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        private void HandleDataSent(object sender, ConnectionArgs e)
        {
            DataSent?.Invoke(sender, e);
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
        /// Unsubscribe from currently subscribed system.
        /// </summary>
        public void UnsubscribeToSystem()
        {
            subSystemHost = null;
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
            foreach (var connection in connectionsCopy)
            {
                connection.Send("Server shutting down. Disconnecting...");
                connection.Disconnect();
            }
        }
    }
}