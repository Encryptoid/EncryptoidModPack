using Eleon.Modding;
using EmpyrionModdingFramework.Teleport;
using System.Collections.Generic;
using System.IO;

namespace AdminTetherport
{
    public class AdminTetherportFormatter
    {
        public static string FormatAttpMessage(List<PlayerInfo> players, PlayerLocationRecord existingLocationRecord)
        {
            var uiString = $"Click one of the below players from the online player list. A Tether will be created at your current " +
                           $"location and you will be teleported to the player.\n" +
                           $"If you tetherport to someone your untether will be overwritten to your current location.\n" +
                           $"Feel free to untether and then admin tetherport if you do not wish to lose your existing tether.\n\n";

            if (existingLocationRecord != null)
            { //Has untether
                uiString += FormatPlayerLocation("UNTETHER", AdminTetherportHandler.UntetherLinkId, existingLocationRecord.Playfield, existingLocationRecord.PosX,
                    existingLocationRecord.PosX, existingLocationRecord.PosZ);
            }

            foreach (var player in players)
            {
                uiString += FormatPlayerLocation(player.playerName, player.entityId, player.playfield, player.pos.x,
                    player.pos.y, player.pos.z);
            }

            return uiString;
        }

        public static string FormatPlayerLocation(string playerName, int entityId, string playfield, float posX,
            float posY, float posZ)
        {
            return $"<link=\"{entityId}\"><indent=5%><line-height=150%>{playerName} | {entityId} | {playfield} | X:{posX} | Y:{posY} | Z:{posZ}</line-height></indent></link>\n";
        }
    }
}
