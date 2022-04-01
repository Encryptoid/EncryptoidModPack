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
    }
}
