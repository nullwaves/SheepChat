using SheepChat.Server.Sessions;
using System;

namespace SheepChat.Server.SessionStates
{
    internal class RegistrationState : SessionState
    {
        private int substate;

        private string[] substates = {
            "Enter a username:",
            "Enter a password:",
            "Enter password again:"
        };

        private string _username = "";
        private string _password = "";

        public RegistrationState(Session session) : base(session)
        {
            Session.Write(substates[substate]);
        }

        public override void ProcessInput(string command)
        {
            string input = string.IsNullOrEmpty(command) ? null : command;
            if (input == null) return;
            switch (substate)
            {
                case 0:
                    _username = command;
                    substate++;
                    break;
                case 1:
                    _password = command;
                    substate++;
                    break;
                case 2:
                    if(_password == command)
                    {
                        //var user = UserRepository.CreateUser(_username, _password);
                        //Session.User = user;
                        //Session.State = new ChattingState(Session);
                        Session.Write();
                        return;
                    }
                    else
                    {
                        Session.Write("Passwords did not match!" + Environment.NewLine);
                        substate--;
                    }
                    break;
                default:
                    return;
            }
            Session.Write(substates[substate]);
        }
    }
}