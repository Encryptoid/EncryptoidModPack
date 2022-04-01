using AdminTetherport;
using Eleon.Modding;
using System.Collections.Generic;
using EmpyrionModdingFramework.Teleport;

namespace Retriever
{
    internal static class RetrieverFormatter
    {
        public static string FormatRetrieveMessage(List<PlayerInfo> players)
        {
            var uiString = $"Click one of the below players from the online player list to teleport them to your location!\n\n";

            foreach (var player in players)
            {
                uiString += AdminTetherportFormatter.FormatPlayerLocation(player.playerName, player.entityId, player.playfield, player.pos.x, player.pos.y, player.pos.z);
            }

            return uiString;
        }

        public static string FormatReturnMessage(List<PlayerLocationRecord> untethers)
        {
            var uiString = $"The below players have been retrieved in the past. Click on one of the untethers to return them to it.\n\n";

            foreach (var untether in untethers)
            {
                uiString += $"{FormatPlayerLocation(untether)}\n";
            }

            uiString += "\n";

            return uiString;
        }

        private static string FormatPlayerLocation(PlayerLocationRecord record)
        {
            return $"<link=\"{record.EntityId}\"><indent=15%><line-height=150%>{record.PlayerName} | {record.Playfield} | {record.PosX} | {record.PosY} | {record.PosZ}</line-height></indent></link>";
        }
    }
}
