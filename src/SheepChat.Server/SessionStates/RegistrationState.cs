using SheepChat.Server.Data.Models;
using SheepChat.Server.Sessions;
using System;

namespace SheepChat.Server.SessionStates
{
    internal class RegistrationState : SessionState
    {
        private int substate;

        private readonly string[] substates = {
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
                    if (_password == command)
                    {
                        var user = new User() { Username = _username, Password = _password };
                        var id = User.manager.Create(user);
                        if (id < 0)
                        {
                            Session.Write("<#magenta>Username already taken<#white>" + Environment.NewLine);
                            substate = 0;
                            break;
                        }
                        user = User.manager.Get(id);
                        Session.Write("<#white><#bblack><#normal>");
                        Session.State = new ChattingState(Session);
                        Session.AuthenticateSession(user);
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