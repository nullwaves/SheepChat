﻿using System.Net.Sockets;
using System;
using SheepChat.Server.Interfaces;
using System.Net;
using System.Text;

namespace SheepChat.Server
{
    internal class Connection : IConnection
    {
        private readonly ISubSystem connectionHost;

        #region Connection Information
        public string ID { get; private set; }
        public IPAddress CurrentIPAddress { get; private set; }
        public byte[] Data { get; private set; }
        public StringBuilder Buffer { get; set; }
        public string LastInputTerminator { get; set; }
        public string LastRawInput { get; set; }
        #endregion

        #region Events
        public event EventHandler<ConnectionArgs> ClientDisconnected;
        public event EventHandler<ConnectionArgs> DataReceived;
        public event EventHandler<ConnectionArgs> DataSent;
        #endregion

        #region Privates
        private readonly Socket socket;
        #endregion

        // Constructor for new connections to the server
        public Connection(Socket socket, ISubSystem connectionHost)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.socket = socket;
            this.connectionHost = connectionHost;

            ID = Guid.NewGuid().ToString();
            CurrentIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            Data = new byte[1];
        }

        internal void BeginListen()
        {
            socket.BeginReceive(this.Data, 0, this.Data.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            try
            {
                int charCount = socket.EndReceive(ar);
                if(charCount == 0)
                {
                    OnDisconnect();
                }
                else
                {
                    DataReceived?.Invoke(this, new ConnectionArgs(this));
                    BeginListen();
                }
            }
            catch
            {
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
        public void Send(string data)
        {
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

        // Formatting for chat messages to be sent to the clients
        public string Format(string msg, string type)
        {
            return "<" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "> [" + type.Trim() + "] " + msg;
        }
    }

    public class ConnectionArgs : EventArgs
    {
        public IConnection Connection { get; private set; }
        public ConnectionArgs(IConnection connnection)
        {
            Connection = connnection;
        }
    }
}
