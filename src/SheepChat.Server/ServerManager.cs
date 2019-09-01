using SheepChat.Server.Interfaces;
using System;

namespace SheepChat.Server
{
    public class ServerManager : Manager
    {
        private static readonly ServerManager Singleton = new ServerManager();

        private readonly Server server = new Server();

        // private readonly InputParser parser = new InputParser();

        public DateTime UpTime;

        public static ServerManager Instance
        {
            get
            {
                return Singleton;
            }
        }

        private ServerManager()
        {
            server.ClientConnect += Server_ClientConnect;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.DataReceived += Server_DataReceived;
            server.DataSent += Server_DataSent;

            // parser.InputReceived += Parser_InputReceived;
        }

        private void Server_DataSent(object sender, ConnectionArgs e)
        {
            // Do nothing by default.
        }

        private void Server_DataReceived(object sender, ConnectionArgs e)
        {
            // Process Incoming Data
        }

        private void Server_ClientDisconnected(object sender, ConnectionArgs e)
        {
            UpdateSubSystemHost((ISubSystem)sender, e.Connection.ID + " - Disconnected");
            SessionManager.Instance.OnSessionDisconnect(e.Connection);
        }

        private void Server_ClientConnect(object sender, ConnectionArgs e)
        {
            UpdateSubSystemHost((ISubSystem)sender, e.Connection.ID + " - Connected");
            SessionManager.Instance.OnSessionConnect(e.Connection);
        }

        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting TCP Server...");
            server.SubscribeToSystem(this);
            server.Start();

            SystemHost.UpdateSystemHost(this, "Server started on port " + server.Port);
            UpTime = DateTime.Now;
        }

        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping server...");
            server.Stop();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }
    }
}
