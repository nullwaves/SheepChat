using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;

namespace SheepChat.Server.SessionStates
{
    internal class ChattingState : SessionState
    {
        private static readonly List<Session> Who = new List<Session>();

        public ChattingState(Session session) : base(session)
        {
            Who.Add(session);
            Session.Write("You are now \"<#bold><#red>Chatting...<#normal><#white>\"" + Environment.NewLine);
        }

        public override void ProcessInput(string command)
        {
            string msg = string.Format("{0}: {1}{2}", Session.User.Username, command, Environment.NewLine);
            foreach(Session sess in Who)
            {
                sess?.Write(msg, true);
            }
        }

        public override void OnLeaveState()
        {
            Who.Remove(Session);
        }
    }
}