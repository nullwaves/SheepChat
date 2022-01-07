using SheepChat.Server.Data.Models;
using SheepChat.Server.Sessions;
using System;
using System.Threading;

namespace SheepChat.Server.SessionStates
{
    internal class LoginState : SessionState
    {
        private readonly User _user;

        public LoginState(Session session, User user) : base(session)
        {
            _user = user;
            Session.Write("<#white>Password: <#black><#bblack>");
        }

        public override void ProcessInput(string command)
        {
            User u = User.manager.Authenticate(_user, command);
            if (u != null)
            {
                Session.Write("<#white>Successfully logged in as " + _user.Username + Environment.NewLine);
                Session.State = new ChattingState(Session);
                Session.AuthenticateSession(u);
            }
            else
            {
                Session.Write("<#magenta>Invalid login details<#white>");
                Thread.Sleep(2000);
                Session.Write("<#cls>");
                Session.State = new ConnectedState(Session);
            }
        }
    }
}