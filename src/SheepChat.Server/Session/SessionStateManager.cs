using SheepChat.Server.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace SheepChat.Server.Sessions
{
    /// <summary>
    /// SessionState manager. It does NOT implement system.
    /// </summary>
    public class SessionStateManager : IRecomposable
    {
        private static readonly object Lock = new object();
        private ConstructorInfo defaultStateConstructor;

        private SessionStateManager()
        {
            Recompose();
        }

        /// <summary>
        /// Compose any loose imports and set the default state constructor.
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);

            Type defaultStateType;

            if (States.Length > 0)
            {
                defaultStateType = (from s in States
                                    orderby s.Metadata.StatePriority descending
                                    select s.Value.GetType()).First();
            }
            else
            {
                defaultStateType = typeof(DefaultState);
            }

            defaultStateConstructor = defaultStateType.GetConstructor(new Type[] { typeof(Session) });
        }

        /// <summary>
        /// Singleton instance of a SessionState manager
        /// </summary>
        public static SessionStateManager Instance { get; } = new SessionStateManager();

        /// <summary>
        /// Loosely imported SessionStates and their priority metadata.
        /// </summary>
        [ImportMany]
        public Lazy<SessionState, ExportSessionStateAttribute>[] States { get; set; }

        /// <summary>
        /// Generate an instance of the default state to set for the new session.
        /// </summary>
        /// <param name="session">Session parameter for SessionState construction</param>
        /// <returns></returns>
        public SessionState CreateDefaultState(Session session)
        {
            lock (Lock)
            {
                var paramaters = new object[] { session };
                return (SessionState)defaultStateConstructor.Invoke(paramaters);
            }
        }
    }

    /// <summary>
    /// Default SessionState where nothing happens.
    /// </summary>
    internal class DefaultState : SessionState
    {
        /// <summary>
        /// Constructor for default state
        /// </summary>
        /// <param name="session">Session that is entering this state</param>
        public DefaultState(Session session) : base(session)
        { 
        }
        
        /// <summary>
        /// Default state's handler for everything
        /// </summary>
        /// <param name="command">It doesn't matter.</param>
        public override void ProcessInput(string command)
        { 
        }
    }
}