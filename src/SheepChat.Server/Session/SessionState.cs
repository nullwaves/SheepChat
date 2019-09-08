using System;
using System.Collections.Generic;
using System.Composition;

namespace SheepChat.Server.Sessions
{
    public abstract class SessionState
    {
        protected Session Session { get; set; }
        public SessionState(Session session)
        {
            Session = session;
        }

        public abstract void ProcessInput(string command);

        public virtual void OnLeaveState() {}
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSessionStateAttribute : ExportAttribute
    {
        public int StatePriority { get; set; }

        public ExportSessionStateAttribute(int statePriority)
            : base(typeof(SessionState))
        {
            this.StatePriority = statePriority;
        }

        public ExportSessionStateAttribute(IDictionary<string, object> metadata)
        {
            foreach(var key in metadata.Keys)
            {
                if(key == "statepriority")
                {
                    StatePriority = (int)metadata[key];
                }
            }
        }
    }
}