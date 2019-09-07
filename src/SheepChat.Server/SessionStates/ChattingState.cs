using SheepChat.Server.Sessions;
using System;

namespace SheepChat.Server.SessionStates
{
    internal class ChattingState : SessionState
    {
        public ChattingState(Session session) : base(session)
        {
            Session.Write("You are now \"<#bold><#red>Chatting...<#normal><#white>\"" + Environment.NewLine);
        }

        public override void ProcessInput(string command)
        {
            Session.Write("You: " + command + Environment.NewLine, true);
        }
    }
}