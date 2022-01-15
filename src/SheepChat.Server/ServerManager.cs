using SheepChat.Server.Interfaces;
using System;

namespace SheepChat.Server
{
    /// <summary>
    /// Server manager system for controlling and monitoring the base server and handling related events.
    /// </summary>
    public class ServerManager : Manager
    {
        /// <summary>
        /// Manager system name
        /// </summary>
        public override string Name
        { get { return "Server"; } }

        private readonly Server server = new Server();

        /// <summary>
        /// DateTime at which the server started listening for connections.
        /// </summary>
        public DateTime UpTime;

        /// <summary>
        /// Singleton instance of the server manager to prevent duplicate systems.
        /// </summary>
        public static ServerManager Instance { get; } = new ServerManager();

        /// <summary>
        /// Default constructor for the ServerManager to assign event handlers.
        /// </summary>
        private ServerManager()
        {
            server.ClientConnect += Server_ClientConnect;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.DataReceived += Server_DataReceived;
            server.DataSent += Server_DataSent;
        }

        /// <summary>
        /// Event handler for the when the base server has finished sending data to a connection
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Connection that the data was sent to</param>
        private void Server_DataSent(object sender, ConnectionArgs e)
        {
            // Do nothing by default.
        }

        /// <summary>
        /// Event handler for when the base server received data from a connection.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Connection that has incoming data received</param>
        private void Server_DataReceived(object sender, ConnectionArgs e)
        {
            // Process Incoming Data
            var connection = e.Connection;
            var data = connection.Data;

            SessionManager.Instance.OnDataReceived(connection, data);
        }

        /// <summary>
        /// Event handler for when a connection disconnects from the base server.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Connection that disconnected</param>
        private void Server_ClientDisconnected(object sender, ConnectionArgs e)
        {
            UpdateSubSystemHost((ISubSystem)sender, e.Connection.ID + " - Disconnected");
            SessionManager.Instance.OnSessionDisconnect(e.Connection);
        }

        /// <summary>
        /// Event handler for when a connection is made on the base server.
        /// </summary>
        /// <param name="sender"><see cref="ISubSystem"/> that triggered the event</param>
        /// <param name="e">Connection that has connected</param>
        private void Server_ClientConnect(object sender, ConnectionArgs e)
        {
            UpdateSubSystemHost((ISubSystem)sender, e.Connection.ID + " - Connected");
            SessionManager.Instance.OnSessionConnect(e.Connection);
        }

        /// <summary>
        /// Start the server manager system.
        /// </summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting TCP Server...");
            server.SubscribeToSystem(this);
            server.Start();

            SystemHost.UpdateSystemHost(this, "Server started on port " + server.Port);
            UpTime = DateTime.Now;
        }

        /// <summary>
        /// Stop the server manager system.
        /// </summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping server...");
            server.Stop();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }
    }

    /// <summary>
    /// Instance exporter for the <see cref="ServerManager"/> system.
    /// </summary>
    [ExportInstance]
    public class ServerManagerInstance : InstanceExporter
    {
        /// <summary>
        /// Singleton instance of the <see cref="ServerManager"/>
        /// </summary>
        public override ISystem Instance => ServerManager.Instance;

        /// <summary>
        /// Instance type of <see cref="ServerManager"/>
        /// </summary>
        public override Type InstanceType => typeof(ServerManager);
    }
}