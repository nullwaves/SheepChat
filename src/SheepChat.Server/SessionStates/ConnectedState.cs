using System;
using SheepChat.Server.Sessions;

namespace SheepChat.Server.SessionStates
{
    [ExportSessionState(100)]
    public class ConnectedState : SessionState
    {
        public ConnectedState(Session session) : base(session)
        {
            if (session == null) return;
            Session.Write("Welcome to SheepChat!" + Environment.NewLine);
            Session.Write("Returning users can login by typing their username; new users can register an account by typing: NEW" + Environment.NewLine);
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
                    Session.State = new LoginState(Session);
                    Session.Write();
                    break;
            }
        }
    }
}
