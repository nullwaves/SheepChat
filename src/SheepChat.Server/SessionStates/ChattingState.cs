using SheepChat.Server.Rooms;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;

namespace SheepChat.Server.SessionStates
{
    /// <summary>
    /// The main SessionState after being authenticated by login or registration, chat with other users.
    /// </summary>
    public class ChattingState : SessionState
    {
        /// <summary>
        /// List of all Sessions in the chatting state.
        /// </summary>
        public static Dictionary<string, Session> Who { get; } = new Dictionary<string, Session>();

        /// <summary>
        /// Constructor for a Session joining the chatting state.
        /// </summary>
        /// <param name="session"></param>
        public ChattingState(Session session) : base(session)
        {
            Who.Add(session.ID, session);
            Session.Write("You are now \"<#bold><#red>Chatting...<#normal><#white>\"" + Environment.NewLine);
            Session.Write("<#svcur>");
        }

        /// <summary>
        /// Handlie incoming messages from the session and forward them onto the other sessions
        /// </summary>
        /// <param name="command">Message being sent</param>
        public override void ProcessInput(string command)
        {
            if (command.Length < 1 || Environment.NewLine.Contains(command)) return;
            RoomManager.Instance.ProcessMessage(Session, command);
        }

        /// <summary>
        /// Session left state, remove it from the connected list.
        /// </summary>
        public override void OnLeaveState()
        {
            Who.Remove(Session.ID);
        }
    }
}