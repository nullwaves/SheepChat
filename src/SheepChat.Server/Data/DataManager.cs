using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

namespace SheepChat.Server.Data
{
    public class DataManager : Manager
    {
        [ImportMany]
        public List<IDocumentStorageProvider> DocumentStorageProviders { get; set; }

        private static IDocumentStorageProvider configuredDocumentStorageProvider;

        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");
            configuredDocumentStorageProvider = (from provider in DocumentStorageProviders
                                                 select provider).FirstOrDefault();
            string bootmsg = string.Format("Using Document Storage Provider: {0}", configuredDocumentStorageProvider.Name);
            SystemHost.UpdateSystemHost(this, bootmsg);
            configuredDocumentStorageProvider.Prepare();
            SystemHost.UpdateSystemHost(this, "Started");
        }

        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");
            SystemHost.UpdateSystemHost(this, "Stopped.");
        }

        public static IDocumentSession<T> OpenDocumentSession<T>()
        {
            return configuredDocumentStorageProvider.OpenDocumentSession<T>();
        }
    }
}
