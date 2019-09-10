using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;

namespace SheepChat.Server
{
    public class ServerManager : Manager
    {
        public override string Name { get { return "Server"; } }

        private readonly Server server = new Server();

        private readonly InputSanitizer sanitizer = new InputSanitizer();

        public DateTime UpTime;

        public static ServerManager Instance { get; } = new ServerManager();

        private ServerManager()
        {
            server.ClientConnect += Server_ClientConnect;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.DataReceived += Server_DataReceived;
            server.DataSent += Server_DataSent;

            sanitizer.InputReceived += Sanitizer_InputReceived;
        }

        private void Server_DataSent(object sender, ConnectionArgs e)
        {
            // Do nothing by default.
        }

        private void Server_DataReceived(object sender, ConnectionArgs e)
        {
            // Process Incoming Data
            var connection = e.Connection;
            var data = connection.Data;

            var buffer = new List<byte>();
            foreach (byte b in data)
            {
                switch (b)
                {
                    case 10:
                    case 13:
                    case byte n when (n > 31 && n < 127):
                        buffer.Add(b);
                        break;
                    case 8:
                    case 127:
                        if (connection.Buffer.Length > 0)
                        {
                            connection.Buffer.Remove(connection.Buffer.Length - 1, 1);
                        }
                        break;
                }
            }

            sanitizer.OnDataReceived(connection, buffer.ToArray());
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

        private void Sanitizer_InputReceived(object sender, ConnectionArgs args, string input)
        {
            SessionManager.Instance.OnInputReceived(args.Connection, input);
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

    [ExportInstance]
    public class ServerManagerInstance : InstanceExporter
    {
        public override ISystem Instance => ServerManager.Instance;

        public override Type InstanceType => typeof(ServerManager);
    }
}
