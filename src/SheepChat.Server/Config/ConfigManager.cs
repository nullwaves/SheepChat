using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace SheepChat.Server.Config
{
    /// <summary>
    /// Manager for loading and saving configurations.
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Active server configuration settings.
        /// </summary>
        public static ServerConfig Current;

        /// <summary>
        /// Load server config from file.
        /// </summary>
        /// <param name="file">JSON Configuration file.</param>
        public static void Load(string file = "config.json")
        {
            try
            {
                var json = File.ReadAllText(file);
                Current = JsonConvert.DeserializeObject<ServerConfig>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading config file from {file}: {e.Message}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                Current = new ServerConfig();
                Save(file);
            }
        }

        /// <summary>
        /// Save server config to file.
        /// </summary>
        /// <param name="file">File to save to.</param>
        public static void Save(string file = "config.json")
        {
            var json = JsonConvert.SerializeObject(Current, Formatting.Indented);
            try
            {
                File.WriteAllText(file, json);
                Console.WriteLine($"Saved configuration to '{new FileInfo(file).FullName}'");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving config file to {file}: {e.Message}");
                Console.Write(json);
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
    }
}