using Eleon.Modding;
using EmpyrionModdingFramework.Teleport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework
{
    public static class PlayerInfoExtensions
    {
        public static PlayerLocationRecord ToPlayerLocationRecord(this PlayerInfo playerInfo)
        {
            return new PlayerLocationRecord()
            {
                EntityId = playerInfo.entityId,
                PlayerName = playerInfo.playerName,
                SteamId = playerInfo.steamId,
                Playfield = playerInfo.playfield,
                PosX = playerInfo.pos.x,
                PosY = playerInfo.pos.y,
                PosZ = playerInfo.pos.z,
                RotX = playerInfo.rot.x,
                RotY = playerInfo.rot.y,
                RotZ = playerInfo.rot.z
            };
        }

        public static bool IsSeated(this PlayerInfo playerInfo)
        { 
            //Forgive me coding gods, for I have sinned. It was the only way..
            //If a players coordinates are a whole number, they are seated.
            return playerInfo.pos.x % 1 == 0;
        }

        public static bool IsAdmin(this PlayerInfo playerInfo)
        {
            return playerInfo.permission >= (int)PlayerPermission.Admin;
        }

        public static bool Y(this char x)
        {
            return x == 'Y';
        }

        public static bool N(this char x)
        {
            return x == 'N';
        }

        public static bool Matches(this PVector3 p1, PVector3 p2)
        {
            return p1.x == p2.x &&
                p1.y == p2.y &&
                p1.z == p2.z;
        }
    }
}
