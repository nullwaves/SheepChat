using System;
using SheepChat.Server.Data.Repositories;
using SheepChat.Server.Sessions;

namespace SheepChat.Server.SessionStates
{
    [ExportSessionState(100)]
    public class ConnectedState : SessionState
    {
        public ConnectedState(Session session) : base(session)
        {
            if (session == null) return;
            Session.Write("<#red>Welcome to SheepChat!" + Environment.NewLine);
            Session.Write("<#white>Returning users can login by typing their username; new users can register an account by typing: <#cyan><#bold>new<#white><#normal>" + Environment.NewLine);
            Session.Write(">");
        }

        public ConnectedState() : this(null) { }

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
                    var user = UserRepository.FindByUsername(command);
                    if (user != null)
                    {
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
