using SheepChat.Server.Config;
using SheepChat.Server.Data.Models;
using SheepChat.Server.Sessions;
using System;
using System.Linq;

namespace SheepChat.Server.SessionStates
{
    /// <summary>
    /// SessionState for a freshly connected session, ovverides the default state.
    /// To ovveride this as the defualt state, see <see cref="ExportSessionStateAttribute"/>
    /// </summary>
    [ExportSessionState(100)]
    public class ConnectedState : SessionState
    {
        /// <summary>
        /// Text to display to user on initial connection.
        /// </summary>
        private static readonly string[] SplashText = ConfigManager.LoadSplashScreen();

        /// <summary>
        /// Constructor for sessionstate, welcome the connection and determine their user existence.
        /// </summary>
        /// <param name="session">Freshly connected session</param>
        public ConnectedState(Session session) : base(session)
        {
            if (session == null) return;
            foreach (string line in SplashText)
            {
                Session.Write(line + Environment.NewLine);
            }
            Session.Write("<#red>Welcome to SheepChat!" + Environment.NewLine);
            Session.Write("<#white>Returning users can login by typing their username; new users can register an account by typing: <#cyan><#bold>new<#white><#normal>" + Environment.NewLine);
            Session.Write(">");
        }

        /// <summary>
        /// Generic constructor because MEF breaks without it.
        /// </summary>
        public ConnectedState() : this(null) { }

        /// <summary>
        /// Handle the first initial commands from a user to determine whether they are logging in, registering, or quitting.
        /// </summary>
        /// <param name="command">Input from connection</param>
        public override void ProcessInput(string command)
        {
            var args = command.Split(new char[] { ' ' });
            string i = args.Length > 0 ? args[0] : string.Empty;
            switch (i.ToLower())
            {
                case "new":
                    Session.State = new RegistrationState(Session);
                    Session.Write();
                    break;

                case "":
                    break;

                case "quit":
                case "close":
                case "exit":
                    Session.Connection.Disconnect();
                    break;

                default:
                    var user = User.manager.FindByUsername(command);
                    if (user != null)
                    {
                        var loggedInAlready = (from s in SessionManager.Instance.Sessions.Values.ToList()
                                               where s.User != null &&
                                               s.User.ID.Equals(user.ID)
                                               select s)
                                               .FirstOrDefault();
                        if (loggedInAlready != null)
                        {
                            Session.Write("<#magenta>User is already logged in.<#white>" + Environment.NewLine);
                            Session.Write(">");
                            return;
                        }
                        Session.State = new LoginState(Session, user);
                    }
                    else
                    {
                        Session.Write("<#magenta>Cannot find user with that username." + Environment.NewLine);
                        Session.Write("<#white>>");
                    }
                    break;
            }
        }
    }
}