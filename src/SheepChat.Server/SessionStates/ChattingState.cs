using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;

namespace SheepChat.Server.SessionStates
{
    internal class ChattingState : SessionState
    {
        public static Dictionary<string, Session> Who { get; } = new Dictionary<string, Session>();

        public ChattingState(Session session) : base(session)
        {
            Who.Add(session.ID, session);
            Session.Write("You are now \"<#bold><#red>Chatting...<#normal><#white>\"" + Environment.NewLine);
            Session.Write("<#svcur>");
        }

        public override void ProcessInput(string command)
        {
            if (command.Length < 1) return;
            string msg = string.Format("{0}: {1}{2}", Session.User.Username, command, Environment.NewLine);
            foreach (Session sess in Who.Values)
            {
                sess?.Write(msg, true);
            }
        }

        public override void OnLeaveState()
        {
            Who.Remove(Session.ID);
        }
    }
}