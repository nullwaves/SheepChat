using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Sheep.Logging;
using Common;
using System;

namespace SheepsTelnetChat
{
    class Connection
    {
        static object BigLock = new object();
        Socket socket;
        public StreamReader Reader;
        public StreamWriter Writer;
        static ArrayList connections = new ArrayList();
        static string ip;
        public bool loggedin = false;
        public string loggedinname = string.Empty;
        public int warninglevel = 0;
        string mode = "none";
        bool isserver = false;
        Thread myThread;

        static Logger serverlog;
        static Dictionary<string, string> userdata = new Dictionary<string, string>();

        public Connection(Logger slog, Dictionary<string, string> udata)
        {
            serverlog = slog;
            userdata = udata;
            isserver = true;
        }

        public Connection(Socket socket)
        {
            this.socket = socket;

            ip = socket.RemoteEndPoint.ToString();
            Reader = new StreamReader(new NetworkStream(socket, false));
            Writer = new StreamWriter(new NetworkStream(socket, true));
            myThread = new Thread(ClientLoop);
            myThread.Start();
        }

        void ClientLoop()
        {
            try
            {
                lock (BigLock)
                {
                    OnConnect();
                }
                while (true)
                {
                    lock (BigLock)
                    {
                        foreach (Connection conn in connections)
                        {
                            conn.Writer.Flush();
                        }
                    }
                    string line = Reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    lock (BigLock)
                    {
                        string input = Server.ReplaceBackspace(line.Trim());
                        if (input.Length > 1) ProcessLine(input);
                    }
                }
            }
            finally
            {
                lock (BigLock)
                {
                    OnDisconnect(false);
                }
            }
        }

        void OnConnect()
        {
            serverlog.Append(ip + " has connected", "user");
            WriteToStream(this, "Welcome!");
            WriteToStream(this, connections.Count + " users are online.");
            connections.Add(this);
            Login(string.Empty);
        }

        private void Login(string data)
        {
            if (mode.Equals("none"))
            {
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
                    if (data != string.Empty)
                    {
                        WriteToStream(this, "Not a valid option! Try Again!");
                        UpWarning();
                    }
                    WriteToStream(this, "Please Choose an Option:");
                    WriteToStream(this, "register");
                    WriteToStream(this, "login");
                }
            }
            else if (mode.Equals("login"))
            {
                if (loggedin == false)
                {
                    if (loggedinname != string.Empty)
                    {
                        if (EncryptionHelper.Encrypt(data) == userdata[loggedinname])
                        {
                            serverlog.Append(loggedinname + " successfully signed in from " + ip, "user");
                            WriteToStream(this, "Successfully Logged in as " + loggedinname + "!");
                            loggedin = true;
                            SendToAll(format(loggedinname + " logged in.", new Room().Name));
                        }
                        else if (data == "!back")
                        {
                            loggedinname = string.Empty;
                        }
                        else
                        {
                            WriteToStream(this, "Wrong Password!");
                            UpWarning();
                        }
                    }
                    else if (data == string.Empty)
                    {
                        WriteToStream(this, "Enter your username:");
                    }
                    else
                    {
                        if (userdata.ContainsKey(data))
                        {
                            loggedinname = data;
                            serverlog.Append(ip + " is attempting to login with username " + data, "login");
                            WriteToStream(this, "Enter Your Password:");
                        }
                        else
                        {
                            WriteToStream(this, "Username does not exist!");
                        }
                    }
                }
            }
            else if (mode.Equals("register"))
            {
                if (!loggedin)
                {
                    if (loggedinname != string.Empty)
                    {
                        if (!userdata.ContainsKey(loggedinname))
                        {
                            userdata.Add(loggedinname, EncryptionHelper.Encrypt(data.Trim()));
                            serverlog.Append(ip + " registered with username " + loggedinname, "user");
                            Server.SaveUserData(userdata);
                            WriteToStream(this, "Account created with username " + loggedinname + " and password " + data.Trim());
                            mode = "login";
                            Login(data.Trim());
                        }
                    }
                    else
                    {
                        if (data == string.Empty)
                        {
                            WriteToStream(this, "Enter Username: ");
                        }
                        else
                        {
                            if (!userdata.ContainsKey(data))
                            {
                                loggedinname = data;
                                WriteToStream(this, "Account Name \"" + data + "\" is available!");
                                WriteToStream(this, "Enter Password: ");
                            }
                            else
                            {
                                WriteToStream(this, "Username already exists!");
                            }
                        }
                    }
                }
            }
        }

        private void UpWarning()
        {
            warninglevel++;
            WriteToStream(this, "Your warning level has been increased!");
            if (warninglevel > 4)
            {
                WriteToStream(this, "You've been kicked for being warned too many times!");
                OnDisconnect(true);
            }
        }

        void OnDisconnect(bool kick)
        {
            if (myThread.ThreadState == ThreadState.Running) myThread.Abort();
            connections.Remove(this);
            socket.Close();
            //if (loggedin)
            //{
            //    serverlog.Append(loggedinname + " (" + ip + ") disconnected", "user");
            //}
            //else serverlog.Append(ip + " disconnected", "user");

            string l = "";
            if (loggedin)
            {
                l += loggedinname + " (" + ip + ")";
            }
            else l += ip;

            if (kick)
            {
                l += " was kicked!";
            }
            else l += " has disconnected";

            serverlog.Append(l, "user");
        }

        public void ProcessLine(string input)
        {
            if (input.StartsWith("/") == false)
            {
                if (loggedin == false)
                {
                    Login(input);
                    serverlog.Append(ip + ": " + input, "login");
                }
                else
                {
                    serverlog.Append(loggedinname + ": " + input);
                    SendToAll(format(loggedinname + ": " + input, new Room().Name));

                }
            }
            else
            {
                if (!loggedin)
                {
                    if (!isserver)
                    {
                        serverlog.Append(loggedinname + " tried command " + input);
                    }
                }
                else
                {
                    string[] args = input.Substring(1).Split(' ');
                    if (args.Length < 1) Login("");
                    serverlog.Append(ip + " tried command " + input + " with " + args.Length + " arguments");
                    switch (args[0])
                    {
                        case "help":
                            WriteToStream(this, "you typed /help");
                            break;
                        case "list":
                            string lists = "";
                            foreach (Connection c in connections)
                            {
                                lists += c.loggedinname + ", ";
                            }
                            lists = lists.Substring(0, lists.Length - 2);
                            WriteToStream(this, "Online Users: " + lists);
                            break;
                        case "quit":
                            this.OnDisconnect(false);
                            break;
                        default:
                            WriteToStream(this, "Invalid Command");
                            break;
                    }
                }
            }
        }

        public string format(string msg, string type)
        {
            return "<" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "> [" + type.Trim() + "] " + msg;
        }

        public static void WriteToStream(Connection c, string msg)
        {
            try
            {
                c.Writer.WriteLine(msg);
            }
            catch (IOException e)
            {
                serverlog.Append(e.ToString());
                WriteToStream(c, msg);
            }
        }

        public void SendToAll(string msg)
        {
            foreach (Connection c in connections)
            {
                if (c.loggedin) WriteToStream(c, msg);
            }
        }
    }
}
