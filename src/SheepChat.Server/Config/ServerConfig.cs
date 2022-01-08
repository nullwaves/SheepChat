namespace SheepChat.Server.Config
{
    /// <summary>
    /// Server Configuration
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Name of storage provider to use.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Connection string/filepath for database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Port to run the TCP server on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Constructor for ServerConfig with defaults.
        /// </summary>
        /// <param name="dbProvider">Name of the StorageProvider to use.</param>
        /// <param name="connectionString">Connection string to use.</param>
        /// <param name="port">Port to allow connections on.</param>
        public ServerConfig(string dbProvider = "LiteDb", string connectionString = "Data Source=.\\db.sqlite3;Version=3;", int port = 23)
        {
            Database = dbProvider;
            ConnectionString = connectionString;
            Port = port;
        }
    }
}