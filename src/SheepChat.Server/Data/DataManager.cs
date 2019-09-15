using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SheepChat.Server.Data
{
    /// <summary>
    /// Database manager system
    /// </summary>
    public class DataManager : Manager, IRecomposable
    {
        /// <summary>
        /// Manager System name
        /// </summary>
        public override string Name { get { return "Database"; } }

        /// <summary>
        /// Singleton instance of this manager.
        /// </summary>
        public static DataManager Instance { get; } = new DataManager();

        /// <summary>
        /// Loosely imported document storage providers
        /// </summary>
        [ImportMany]
        public IEnumerable<IDocumentStorageProvider> DocumentStorageProviders { get; set; }

        private static IDocumentStorageProvider configuredDocumentStorageProvider;

        /// <summary>
        /// Constructor for DataManager, triggers recomposition
        /// </summary>
        public DataManager()
        {
            Recompose();
        }

        /// <summary>
        /// Start the data manager system. Selects the storage provider to use
        /// </summary>
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

        /// <summary>
        /// Stop the data manager system.
        /// </summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");
            SystemHost.UpdateSystemHost(this, "Stopped.");
        }

        /// <summary>
        /// Open a new document session for accessing the document storage
        /// </summary>
        /// <typeparam name="T">Document Type</typeparam>
        /// <returns>An <see cref="IDocumentSession{T}"/> from the configured Document Storage Provider</returns>
        public static IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase
        {
            return configuredDocumentStorageProvider.OpenDocumentSession<T>();
        }

        /// <summary>
        /// Compose any loose imports
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);
        }
    }

    /// <summary>
    /// Instance exporter for the Data Manager System
    /// </summary>
    [ExportInstance]
    public class DataManagerInstance : InstanceExporter
    {
        /// <summary>
        /// Singleton Instance of the DataManager
        /// </summary>
        public override ISystem Instance => DataManager.Instance;

        /// <summary>
        /// Instance type of DataManager
        /// </summary>
        public override Type InstanceType => typeof(DataManager);
    }
}
