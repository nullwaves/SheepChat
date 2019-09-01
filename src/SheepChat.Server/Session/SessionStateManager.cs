using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;

namespace SheepChat.Server.Sessions
{
    public class SessionStateManager : IRecomposable
    {
        private static readonly object Lock = new object();

        private static readonly SessionStateManager Singleton = new SessionStateManager();

        private ConstructorInfo defaultStateConstructor;

        private SessionStateManager()
        {
            Recompose();
        }

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

    internal class DefaultState : SessionState
    {

        public DefaultState(Session session) : base(session)
        { 
        }
        
        public override void ProcessInput(string command)
        { 
        }
    }
}