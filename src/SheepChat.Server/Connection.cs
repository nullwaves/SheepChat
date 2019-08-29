using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Sheep.Logging;
using Common;
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
        #endregion

        #region Event Handlers
        public event EventHandler<ConnectionArgs> ClientDisconnected;
        public event EventHandler<ConnectionArgs> DataReceived;
        public event EventHandler<ConnectionArgs> DataSent;
        #endregion

        #region Privates
        private readonly Socket socket;
        #endregion

        /* +++++ TODO: Eliminate between comments +++++ */
        // TODO: Remove neccesity of defaults
        // Statics
        private static Dictionary<string, string> userdata = new Dictionary<string, string>();

        // Sockets & Streams
        
        private StreamReader Reader;
        private bool loggedin = false;
        private string loggedinname = string.Empty;
        private string mode = "none";
        /* ++++++++++++++++++++++++++++++++++++++++++++ */

        // Constructor for a fake connection from the server to process commands
        // TODO: Remove neccesity
        internal Connection(Dictionary<string, string> udata)
        {
            userdata = udata;
        }

        // Constructor for new connections to the server
        public Connection(Socket socket, ISubSystem connectionHost)
        {
            this.socket = socket;
            this.connectionHost = connectionHost;

            ID = Guid.NewGuid().ToString();
            CurrentIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            Data = new byte[1];
        }

        // Loop for collecting input from clients to be processed
        void ClientLoop()
        {
            try
            {
                OnConnect();
                while (!Reader.EndOfStream)
                {
                    string line = Reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string input = Server.ReplaceBackspace(line.Trim());
                    if (input.Length > 1) ProcessLine(input);
                }
            }
            finally
            {
                OnDisconnect();
            }
        }

        // Things to do when the client first connects
        void OnConnect()
        {
            Send("Welcome!");
            Login(string.Empty);
        }

        // Process input from the login state
        // TODO: Refactor as ConnectionStates
        private void Login(string data)
        {
            if (mode.Equals("none"))
            {
                //  User should be selecting an option
                if (data.Equals("login"))
                {
                    mode = "login";
                    Login(string.Empty);
                }
                else if (data.Equals("register"))
                {
                    mode = "register";
                    Login(string.Empty);
                }
                else
                {
                    // No option has been selected yet.
                    if (data != string.Empty)
                    {
                        Send("Not a valid option! Try Again!");
                    }
                    Send("Please Choose an Option:");
                    Send("register");
                    Send("login");
                }
            }
            else if (mode.Equals("login"))
            {
                // User is logging in to an existing account
                if (loggedin == false)
                {
                    if (loggedinname != string.Empty)
                    {
                        if (EncryptionHelper.Encrypt(data) == userdata[loggedinname])
                        {
                            Send("Successfully Logged in as " + loggedinname + "!");
                            loggedin = true;
                            //SendToAll(format(loggedinname + " logged in.", new Room().Name));
                        }
                        else if (data == "!back")
                        {
                            loggedinname = string.Empty;
                        }
                        else
                        {
                            Send("Wrong Password!");
                        }
                    }
                    else if (data == string.Empty)
                    {
                        Send("Enter your username:");
                    }
                    else
                    {
                        if (userdata.ContainsKey(data))
                        {
                            loggedinname = data;
                            Send("Enter Your Password:");
                        }
                        else
                        {
                            Send("Username does not exist!");
                        }
                    }
                }
            }
            else if (mode.Equals("register"))
            {
                // User is attempting to register a new account
                if (!loggedin)
                {
                    if (loggedinname != string.Empty)
                    {
                        if (!userdata.ContainsKey(loggedinname))
                        {
                            userdata.Add(loggedinname, EncryptionHelper.Encrypt(data.Trim()));
                            Server.SaveUserData(userdata);
                            Send("Account created with username " + loggedinname + " and password " + data.Trim());
                            mode = "login";
                            Login(data.Trim());
                        }
                    }
                    else
                    {
                        if (data == string.Empty)
                        {
                            Send("Enter Username: ");
                        }
                        else
                        {
                            if (!userdata.ContainsKey(data))
                            {
                                loggedinname = data;
                                Send("Account Name \"" + data + "\" is available!");
                                Send("Enter Password: ");
                            }
                            else
                            {
                                Send("Username already exists!");
                            }
                        }
                    }
                }
            }
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
            var bytes = Encoding.GetEncoding(437).GetBytes(data);
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

        // General command processing
        // TODO: Refactor as a CommandManager component of a ConnectionState
        public void ProcessLine(string input)
        {
            if (input.StartsWith("/") == false)
            {
                // Client is just sending text, pass on to others
                if (!loggedin)
                {
                    Login(input);
                }
                else Send(format(input, new Room().Name));
            }
            else
            {
                // Clint is trying to proccess a command
                string[] args = input.Substring(1).Split(' ');
                if (args.Length < 1) Login("");
                switch (args[0])
                {
                    case "help":
                        Send("you typed /help"); // Great helpful command here.
                        break;
                    case "list":
                        throw new NotImplementedException();
                        //string lists = "";
                        //lists = lists.Substring(0, lists.Length - 2);
                        //Send("Online Users: " + lists);
                        //break;
                    case "quit":
                        this.OnDisconnect();
                        break;
                    default:
                        Send("Invalid Command");
                        break;
                }
            }
        }

        // Formatting for chat messages to be sent to the clients
        public string format(string msg, string type)
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
