using SheepChat.Server.Interfaces;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    public class RoomManager : Manager, IRecomposable
    {
        public static RoomManager Instance { get; } = new RoomManager();

        public override string Name => "Rooms";

        public static Dictionary<string, IRoom> RoomList { get; private set; }

        public void Recompose()
        {
            Composer.Compose(this);
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
