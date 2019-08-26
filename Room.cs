using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Threading;
using Sheep.Logging;

namespace SheepsTelnetChat
{
    class Room
    {
        static ArrayList Rooms = new ArrayList();
        private List<Connection> joined = new List<Connection>();
        private List<String> banned = new List<String>();
        public string Name = "Main";
    }
}
