using SheepChat.Server.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SheepChat.Server
{
    /// <summary>
    /// Functional implementation of <see cref="IConnection"/>
    /// </summary>
    public class Connection : IConnection
    {
#pragma warning disable IDE0052
        private readonly ISubSystem connectionHost;
#pragma warning restore IDE0052

        /// <summary>
        /// IAC DO SUPPRESS_LOCAL_ECHO
        /// </summary>
        private static readonly byte[] IACSLE = new byte[] { 0xFF, 0xFD, 0x2D };

        #region Connection Information

        /// <summary>
        /// Connection ID string
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// IPAddress from which the connection is currently connected.
        /// </summary>
        public IPAddress CurrentIPAddress { get; private set; }

        /// <summary>
        /// Input buffer of data being sent by the connection.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Denotes which line terminator this session used the last time it sent data.
        /// </summary>
        public string LastInputTerminator { get; set; }

        /// <summary>
        /// The last raw input sent by the connection.
        /// </summary>
        public string LastRawInput { get; set; }

        #endregion Connection Information

        #region Events

        /// <summary>
        /// Event triggered when a connection is disconnected.
        /// </summary>
        public event EventHandler<ConnectionArgs> ClientDisconnected;

        /// <summary>
        /// Event triggered when data is received from the client.
        /// </summary>
        public event EventHandler<ConnectionArgs> DataReceived;

        /// <summary>
        /// Event triggered when data has been sent to the client.
        /// </summary>
        public event EventHandler<ConnectionArgs> DataSent;

        #endregion Events

        #region Privates

        private readonly Socket socket;

        #endregion Privates

        /// <summary>
        /// Default constructor for a new connection to the server.
        /// </summary>
        /// <param name="socket">Socket that is connecting</param>
        /// <param name="connectionHost">Server hosting the connection</param>
        public Connection(Socket socket, ISubSystem connectionHost)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.socket = socket;
            this.connectionHost = connectionHost;

            ID = Guid.NewGuid().ToString();
            CurrentIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            Data = new byte[32];

            //this.Send(IACSLE);
        }

        /// <summary>
        /// Begin listening asynchronously for data from the connection.
        /// </summary>
        public void BeginListen()
        {
            Data = new byte[32];
            socket.BeginReceive(this.Data, 0, this.Data.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
        }

        /// <summary>
        /// Async event handler for data received from the connection.
        /// </summary>
        /// <param name="ar">AsyncResult of data received</param>
        private void OnDataReceived(IAsyncResult ar)
        {
            try
            {
                int charCount = socket.EndReceive(ar);
                if (charCount == 0)
                {
                    OnDisconnect();
                }
                else
                {
                    DataReceived?.Invoke(this, new ConnectionArgs(this));
                    BeginListen();
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                Console.WriteLine(e.GetType().ToString() + "  " + e.Message);
                OnDisconnect();
            }
        }

        /// <summary>
        /// Close and disconnect the socket connection.
        /// </summary>
        public void Disconnect()
        {
            OnDisconnect();
        }

        /// <summary>
        /// Send raw bytes over the connection.
        /// </summary>
        /// <param name="data">The bytes to send.</param>
        public void Send(byte[] data)
        {
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSendComplete), null);
        }

        /// <summary>
        /// Send string of data over the connection.
        /// </summary>
        /// <param name="data">The string to send.</param>
        public void Send(string data) => Send(data, false);

        /// <summary>
        /// Send string of data over the connection.
        /// </summary>
        /// <param name="data">The string to send.</param>
        /// <param name="bypassFormatter">Flag for pre-formatted data or data that should not be formatted</param>
        public void Send(string data, bool bypassFormatter)
        {
            data = !bypassFormatter ? ANSIShortcodeFormatter.Format(data) : data;
            byte[] bytes;
            bytes = Encoding.GetEncoding(437).GetBytes(data);
            Send(bytes);
        }

        /// <summary>
        /// Async callback when send is completed.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                socket.EndSend(ar);
                DataSent?.Invoke(this, new ConnectionArgs(this));
            }
            catch
            {
                OnDisconnect();
            }
        }

        /// <summary>
        /// Produce paginated output from the buffer.
        /// TODO: Implement buffer
        /// </summary>
        public void ProcessBuffer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects the socket and raises the ClientDisconnected event.
        /// </summary>
        private void OnDisconnect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            ClientDisconnected(this, new ConnectionArgs(this));
        }
    }

    /// <summary>
    /// Connection details for related events and associated handlers.
    /// </summary>
    public class ConnectionArgs : EventArgs
    {
        /// <summary>
        /// Connection involved in the event.
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        /// Construct a new ConnectionArgs with a connection.
        /// </summary>
        /// <param name="connnection">The accused connection</param>
        public ConnectionArgs(IConnection connnection)
        {
            Connection = connnection;
        }
    }
}