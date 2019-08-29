using System;
using System.Collections.Generic;
using System.Collections;

namespace SheepChat.Server
{
    class Room
    {
        static ArrayList Rooms = new ArrayList();
        private List<Connection> joined = new List<Connection>();
        public string Name = "Main";
    }
}
