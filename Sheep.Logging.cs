using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sheep
{
    namespace Logging
    {
        class Logger
        {
            private static StreamWriter _out;

            public Logger(string path)
            {
                if(!Directory.Exists("logs")) Directory.CreateDirectory("logs");
                _out = File.AppendText("logs\\"+path);
                _out.AutoFlush = true;
            }

            public void Append(string message)
            {
                this.Append(message, 1);
            }

            public void Append(string message, int level)
            {
                switch (level)
                {
                    case 2:
                        this.Append(message, "warning");
                        break;
                    case 3:
                        this.Append(message, "error");
                        break;
                    default:
                        this.Append(message, "info");
                        break;
                }
            }

            public void Append(string message, string type)
            {
                lock(this)
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
}
