using SheepChat.Server.Data.Interfaces;
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
        public override string Name
        { get { return "Database"; } }

        /// <summary>
        /// Singleton instance of this manager.
        /// </summary>
        public static DataManager Instance { get; } = new DataManager();

        /// <summary>
        /// Loosely imported  storage providers
        /// </summary>
        [ImportMany]
        public IEnumerable<IStorageProvider> StorageProviders { get; set; }

        private static IStorageProvider configuredStorageProvider;

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
            string bootmsg = string.Format("Using  Storage Provider: {0}", configuredStorageProvider.Name);
            SystemHost.UpdateSystemHost(this, bootmsg);
            configuredStorageProvider.Prepare();
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
        /// Open a new  session for accessing the  storage
        /// </summary>
        /// <typeparam name="T"> Type</typeparam>
        /// <returns>An <see cref="IRepository{T}"/> from the configured  Storage Provider</returns>
        public static IRepository<T> OpenRepository<T>() where T : IModel
        {
            return configuredStorageProvider.OpenRepository<T>();
        }

        /// <summary>
        /// Compose any loose imports
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);

            configuredStorageProvider = (from provider in StorageProviders
                                         select provider).FirstOrDefault();
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