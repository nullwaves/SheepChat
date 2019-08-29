using System;
using System.IO;

namespace Sheep.Logging
{
    internal class Logger
    {
        private static StreamWriter _out;

        public Logger(string path)
        {
            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            _out = File.AppendText("logs\\" + path);
            _out.AutoFlush = true;
        }

        public void Append(string message)
        {
            Append(message, 1);
        }

        public void Append(string message, int level)
        {
            switch (level)
            {
                case 2:
                    Append(message, "warning");
                    break;
                case 3:
                    Append(message, "error");
                    break;
                default:
                    Append(message, "info");
                    break;
            }
        }

        public void Append(string message, string type)
        {
            lock (this)
            {
                string lineout = "<" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "> [" + type.Trim().ToUpper() + "] " + message.Trim();
                _out.WriteLine(lineout);
                Console.WriteLine(lineout);
            }
        }

        public void Dispose()
        {
            _out.Dispose();
        }

    }
}
