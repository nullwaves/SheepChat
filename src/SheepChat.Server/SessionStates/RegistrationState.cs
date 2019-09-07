using SheepChat.Server.Data.Repositories;
using SheepChat.Server.Sessions;
using System;

namespace SheepChat.Server.SessionStates
{
    internal class RegistrationState : SessionState
    {
        private int substate;

        private string[] substates = {
            "Enter a username:",
            "Enter a password:<#bblack><#black>",
            "<#white>Enter password again:<#black>"
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
                        var user = UserRepository.Create(_username, _password);
                        Session.AuthenticateSession(user);
                        Session.Write("<#white><#bblack><#normal>");
                        Session.State = new ChattingState(Session);
                        return;
                    }
                    else
                    {
                        Session.Write("<#magenta>Passwords did not match!<#white>" + Environment.NewLine);
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