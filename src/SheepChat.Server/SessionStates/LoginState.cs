using SheepChat.Server.Sessions;

namespace SheepChat.Server.SessionStates
{
    internal class LoginState : SessionState
    {
        public LoginState(Session session) : base(session)
        {
        }

        public override void ProcessInput(string command)
        {
            throw new System.NotImplementedException();
        }
    }
}