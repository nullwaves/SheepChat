using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace SheepChat.Server.Data
{
    public class DataManager : Manager, IRecomposable
    {
        public override string Name { get { return "Database"; } }

        private static readonly DataManager Singleton = new DataManager();

        public static DataManager Instance => Singleton;

        [ImportMany]
        public IEnumerable<IDocumentStorageProvider> DocumentStorageProviders { get; set; }

        private static IDocumentStorageProvider configuredDocumentStorageProvider;

        public DataManager()
        {
            Recompose();
        }

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

        public static IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase
        {
            return configuredDocumentStorageProvider.OpenDocumentSession<T>();
        }

        public void Recompose()
        {
            Composer.Compose(this);
        }
    }

    [ExportInstance]
    public class DataManagerInstance : InstanceExporter
    {
        public override ISystem Instance => DataManager.Instance;

        public override Type InstanceType => typeof(DataManager);
    }
}
