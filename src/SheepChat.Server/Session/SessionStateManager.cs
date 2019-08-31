using SheepChat.Server.Interfaces;
using System;
using System.Composition;
using System.Reflection;

namespace SheepChat.Server.Session
{
    internal class SessionStateManager : IRecomposable
    {
        private static readonly object Lock = new object();

        private static readonly SessionStateManager Singleton = new SessionStateManager();

        private ConstructorInfo defaultStateConstructor;

        private SessionStateManager()
        {
            Recompose();
        }

        public static SessionStateManager Instance
        {
            get
            {
                return Singleton;
            }
        }

        [ImportMany]
        public Lazy<SessionState, ExportSessionStateAttribute>[] States { get; set; }

        public SessionState CreateDefaultState(Session session)
        {
            lock (Lock)
            {
                var paramaters = new object[] { session };
                return (SessionState)defaultStateConstructor.Invoke(paramaters);
            }
        }
    }
}