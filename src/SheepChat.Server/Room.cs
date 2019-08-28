using System;
using System.Collections.Generic;
using System.Collections;

namespace Sheep.Telnet
{
    class Room
    {
        static ArrayList Rooms = new ArrayList();
        private List<Connection> joined = new List<Connection>();
        private List<String> banned = new List<String>();
        public string Name = "Main";
    }
}
