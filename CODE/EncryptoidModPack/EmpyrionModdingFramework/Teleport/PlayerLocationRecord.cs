using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework.Teleport
{
    public class PlayerLocationRecord
    {
        public PlayerLocationRecord() { } //For CsvHelper
        public PlayerLocationRecord(string playerName, string steamId, int entityId, string playfield, PVector3 pos, PVector3 rot)
        {
            PlayerName = playerName;
            SteamId = steamId;
            EntityId = entityId;
            Playfield = playfield;
            PosX = pos.x;
            PosY = pos.y;
            PosZ = pos.z;
            RotX = rot.x;
            RotY = rot.y;
            RotZ = rot.z;
        }

        public string PlayerName { get; set; }
        public string SteamId { get; set; }
        public int EntityId { get; set; }
        public string Playfield { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }

        public PortalRecord ToLocationRecord(string locationName, char adminYN = 'N')
        {
            return new PortalRecord
            {
                AdminYN = adminYN,
                Name = locationName,
                Playfield = Playfield,
                PosX = PosX,
                PosY = PosY,
                PosZ = PosZ,
                RotX = RotX,
                RotY = RotY,
                RotZ = RotZ
            };
        }
    }
}
