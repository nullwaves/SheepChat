using SheepChat.Server.Data.Models;
using SheepChat.Server.Data.Repositories;
using SheepChat.Server.Sessions;
using System;
using System.Threading;

namespace SheepChat.Server.SessionStates
{
    internal class LoginState : SessionState
    {
        private readonly string _username;


        public LoginState(Session session, string username) : base(session)
        {
            _username = username;
        }

        public override void ProcessInput(string command)
        {
            User u = UserRepository.Authenticate(_username, command);
            if (u != null)
            {
                Session.AuthenticateSession(u);
                Session.Write("<#white>Successfully logged in as " + _username + Environment.NewLine);
                Session.State = new ChattingState(Session);
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