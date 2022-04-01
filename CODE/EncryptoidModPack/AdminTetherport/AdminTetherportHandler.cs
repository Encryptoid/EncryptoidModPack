using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using Tetherport;

namespace AdminTetherport
{
    public class AdminTetherportHandler
    {
        private readonly IDatabaseManager _dbManager;
        private readonly EmpyrionModdingFrameworkBase _modFramework;
        private readonly Action<string> _log;

        public const string ModName = "AdminTetherport";
        public const string AdminTetherportCommand = "admin-tetherport";
        public const string AdminTetherportShorthand = "attp";
        public const int UntetherLinkId = -99;

        public AdminTetherportHandler(IDatabaseManager dbManager, EmpyrionModdingFrameworkBase modFramework, Action<string> logFunc)
        {
            _dbManager = dbManager;
            _modFramework = modFramework;
            _log = logFunc;
        }

        public async Task ShowOnlinePlayers(MessageData messageData)
        {
            var adminPlayer = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            if (adminPlayer == null) return;

            var playerIdList = await _modFramework.QueryPlayerList();

            List<PlayerInfo> allPlayers = new List<PlayerInfo>();
            foreach (var playerId in playerIdList)
            {
                allPlayers.Add(await _modFramework.QueryPlayerInfo(playerId));
            }

            var existingTetherRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(adminPlayer.steamId))?.FirstOrDefault();

            _modFramework.ShowLinkedTextDialog(adminPlayer.entityId, AdminTetherportFormatter.FormatAttpMessage(allPlayers, existingTetherRecord), "Admin Tetherport!", TetherportToPlayer);
        }

        private async void TetherportToPlayer(int buttonIdx, string linkId, string inputContent, int adminId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId))
                return;

            if (string.Equals(linkId, $"{UntetherLinkId}"))
            {
                AdminUntether(adminId);
            }

            var targetPlayerId = int.Parse(linkId);
            var adminPlayer = await _modFramework.QueryPlayerInfo(adminId);

            _dbManager.SaveRecord(TetherportFormatter.FormatTetherportFileName(adminPlayer.steamId), adminPlayer.ToPlayerLocationRecord(),
                clearExisting: true);

            await _modFramework.TeleportPlayerToPlayer(adminPlayer.entityId, targetPlayerId);
            await _modFramework.MessagePlayer(adminPlayer.entityId, $"Created Admin Tetherport tether! Teleported to PlayerId:{targetPlayerId}.", 5);
        }

        private async void AdminUntether(int adminId)
        {
            PlayerInfo player = await _modFramework.QueryPlayerInfo(adminId);
            if (player == null) return;

            var existingRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            if (existingRecord == null)
            {
                _log($"Entity {player.entityId}/{player.playerName} requester Untether but no tether was found.");
                await _modFramework.MessagePlayer(player.entityId, $"No tether was found.", 5, MessagerPriority.Red);
                return;
            }

            await _modFramework.TeleportPlayer(player.entityId, existingRecord.Playfield,
                existingRecord.PosX, existingRecord.PosY, existingRecord.PosZ,
                existingRecord.RotX, existingRecord.RotY, existingRecord.RotZ);
        }
    }
}
