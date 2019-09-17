using SheepChat.Server.Interfaces;
using System.Collections.Generic;
using System;
using SheepChat.Server.Sessions;
using SheepChat.Server.Data.Models;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Room Manager System for loading and handling user events while chatting.
    /// </summary>
    public class RoomManager : Manager, IRecomposable
    {
        /// <summary>
        /// Singleton instance so there is only ever one given RoomManager.
        /// </summary>
        public static RoomManager Instance { get; } = new RoomManager();

        /// <summary>
        /// Manager System Name
        /// </summary>
        public override string Name => "Rooms";

        /// <summary>
        /// Dictionary of Rooms and their reference string.
        /// </summary>
        public static Dictionary<string, IRoom> RoomList { get; private set; }

        /// <summary>
        /// Dictionary of current locations of what room each session is in.
        /// </summary>
        public static Dictionary<string, string> SessionLocations { get; private set; }

        /// <summary>
        /// Compose any loose imports.
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);
        }

        /// <summary>
        /// Forward a message from the session to the proper room.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        public void ProcessMessage(Session session, string message)
        {
            if(SessionLocations.ContainsKey(session.ID))
            {
                var room = GetRoomContainingSession(session);
                var send = $"{session.User.Username}: {message}";
                room.Send(send + Environment.NewLine);
                SystemHost.UpdateSystemHost(this, $"[{room.Name}] {send}");
            }
        }

        /// <summary>
        /// Get the room that the provided session is currently in.
        /// </summary>
        /// <param name="session">Session to locate</param>
        /// <returns>IRoom location of the Session</returns>
        public IRoom GetRoomContainingSession(Session session)
        {
            if(SessionLocations.ContainsKey(session.ID))
            {
                return RoomList[SessionLocations[session.ID]];
            }
            return null;
        }

        /// <summary>
        /// Move a user from their current room to a room with the provided name.
        /// </summary>
        /// <param name="session">Session to be moved</param>
        /// <param name="name">Name of the room to move to</param>
        public static void MoveTo(Session session, string name)
        {
            name = name.ToLower();
            if(!RoomList.ContainsKey(name))
            {
                throw new ArgumentException("Room with that name does not exist.");
            }
            var newRoom = RoomList[name];
            var oldRoom = Instance.GetRoomContainingSession(session);
            oldRoom?.Leave(session);
            newRoom.Join(session);
            if(SessionLocations.ContainsKey(session.ID))
            {
                SessionLocations[session.ID] = newRoom.Name.ToLower();
            }
            else
            {
                SessionLocations.Add(session.ID, newRoom.Name.ToLower());
            }
        }

        /// <summary>
        /// Create a new room if one with the provided name does not already exist.
        /// </summary>
        /// <param name="sender">Session of the user trying to create a room.</param>
        /// <param name="name">Name of the room to create.</param>
        /// <returns></returns>
        public static UserOwnedRoom CreateRoom(Session sender, string name)
        {
            var record = new RoomRecord()
            {
                Name = name,
                Description = "",
                OwnerUserID = sender.User.ID
            };
            var room = RoomList.ContainsKey(name.ToLower()) ? null : new UserOwnedRoom(record);
            if(room != null)
            {
                RoomRepository.Save(room);
                RoomList.Add(room.Name.ToLower(), room);
            }
            return room;
        }

        /// <summary>
        /// Start the Room system.
        /// </summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");
            RoomList.Add("main", new DefaultRoom());
            var num = LoadUserOwnedRooms();
            SystemHost.UpdateSystemHost(this, $"Loaded {num} user owned rooms");
            SystemHost.UpdateSystemHost(this, "Started");
        }

        private int LoadUserOwnedRooms()
        {
            var ret = 0;
            var rooms = RoomRepository.GetAllUserOwnedRooms();
            foreach (var room in rooms)
            {
                RoomList.Add(room.Name.ToLower(), room);
                ret++;
            }
            return ret;
        }

        /// <summary>
        /// Stop the Room system.
        /// </summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");
            foreach (IRoom r in RoomList.Values)
            {
                if(r.GetType().Equals(typeof(UserOwnedRoom)))
                {
                    RoomRepository.Save((UserOwnedRoom)r);
                }
            }
            RoomList.Clear();
            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        private RoomManager()
        {
            Recompose();

            RoomList = new Dictionary<string, IRoom>();
            SessionLocations = new Dictionary<string, string>();

            SessionManager.Instance.SessionAuthenticated += OnSessionAuthenticated;
            SessionManager.Instance.SessionDisconnected += OnSessionDisconnected;
        }

        private void OnSessionDisconnected(Session session)
        {
            if (SessionLocations.ContainsKey(session.ID))
            {
                IRoom room = GetRoomContainingSession(session);
                room?.Leave(session);
                lock(SessionLocations)
                {
                    SessionLocations.Remove(session.ID);
                }
            }
        }

        private void OnSessionAuthenticated(Session session)
        {
            var room = RoomList["main"];
            room.Join(session);
            lock (SessionLocations)
            {
                SessionLocations.Add(session.ID, room.Name.ToLower());
            }
        }
    }

    /// <summary>
    /// Singleton exporter for the Room Manager system.
    /// </summary>
    [ExportInstance]
    public class RoomManagerInstance : InstanceExporter
    {
        /// <summary>
        /// Singleton instance of a RoomManager.
        /// </summary>
        public override ISystem Instance => RoomManager.Instance;

        /// <summary>
        /// Instance type of RoomManager.
        /// </summary>
        public override Type InstanceType => typeof(RoomManager);
    }
}
