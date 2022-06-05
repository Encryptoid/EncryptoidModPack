using AdminTetherport;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetherport;

namespace Retriever
{
    public class RetrieverHandler
    {
        private readonly IDatabaseManager _dbManager;
        private readonly EmpyrionModdingFrameworkBase _modFramework;
        private readonly Action<string> _log;

        public const string ModName = "Retriever";
        public const string RetrieveCommand = "retrieve-tether";
        public const string RetrieveIssueCommand = "retrieve";
        public const string ReturnCommand = "return";

        public RetrieverHandler(IDatabaseManager dbManager, EmpyrionModdingFrameworkBase modFramework, Action<string> logFunc)
        {
            _dbManager = dbManager;
            _modFramework = modFramework;
            _log = logFunc;
        }

        public async Task DialogRetrievePlayer(MessageData messageData)
        { 
            await DialogRetrievePlayer(messageData.SenderEntityId, !messageData.Text.Contains(RetrieveCommand));
        }
        
        private async Task DialogRetrievePlayer(int senderEntityId, bool isIssueCommand)
        {
            var adminPlayer = await _modFramework.QueryPlayerInfo(senderEntityId);
            if (adminPlayer == null) return;

            var playerIdList = await _modFramework.QueryPlayerList();

            List<PlayerInfo> allPlayers = new List<PlayerInfo>();
            foreach (var playerId in playerIdList)
            {
                allPlayers.Add(await _modFramework.QueryPlayerInfo(playerId));
            }

            if (isIssueCommand)
            {
                _modFramework.ShowLinkedTextDialog(adminPlayer.entityId,
                    RetrieverFormatter.FormatRetrieveMessage(allPlayers), "Retrieve Player(Issue)!", RetrievePlayerIssue);
            }
            else
            {
                _modFramework.ShowLinkedTextDialog(adminPlayer.entityId,
                    RetrieverFormatter.FormatRetrieveMessage(allPlayers), "Retrieve Player!", RetrievePlayer);
            }
        }

        private async void RetrievePlayerIssue(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId) || linkId == "-1")
                return;

            var targetPlayerId = int.Parse(linkId);
            var targetPlayer = await _modFramework.QueryPlayerInfo(targetPlayerId);
            var adminPlayer = await _modFramework.QueryPlayerInfo(playerId);

            await _modFramework.TeleportPlayerToPlayer(targetPlayerId, adminPlayer.entityId);
            await _modFramework.MessagePlayer(adminPlayer.entityId, $"Teleported player to your location. EntityId: {targetPlayerId}.", 5);
        }

        private async void RetrievePlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId) || linkId == "-1")
                return;

            var targetPlayerId = int.Parse(linkId);
            var targetPlayer = await _modFramework.QueryPlayerInfo(targetPlayerId);
            var adminPlayer = await _modFramework.QueryPlayerInfo(playerId);

            _dbManager.SaveRecord(TetherportFormatter.FormatTetherportFileName(targetPlayer.steamId), targetPlayer.ToPlayerLocationRecord(), true);

            await _modFramework.TeleportPlayerToPlayer(targetPlayerId, adminPlayer.entityId);
            await _modFramework.MessagePlayer(adminPlayer.entityId, $"Teleported player to your location. EntityId: {targetPlayerId}.", 5);

            await DialogRetrievePlayer(adminPlayer.entityId, false);
        }


        public async Task DialogReturnPlayer(MessageData messageData)
        {
            await DialogReturnPlayer(messageData.SenderEntityId);    
        }

        private async Task DialogReturnPlayer(int senderEntityId)
        {
            var adminPlayer = await _modFramework.QueryPlayerInfo(senderEntityId);
            if (adminPlayer == null) return;

            var untethers = _dbManager.LoadAllRecords<PlayerLocationRecord>("Tethers", _log);
            _modFramework.ShowLinkedTextDialog(adminPlayer.entityId, RetrieverFormatter.FormatReturnMessage(untethers), "Return Player", ReturnPlayer);
        }

        private async void ReturnPlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId) || linkId == "-1")
                return;

            var targetPlayerId = int.Parse(linkId);
            var targetPlayer = await _modFramework.QueryPlayerInfo(targetPlayerId);

            var existingRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(targetPlayer.steamId))?.FirstOrDefault();

            if (existingRecord == null)
            {
                await _modFramework.MessagePlayer(playerId, $"Error. Could not return player.", 5, MessagerPriority.Red);
                return;
            }

            await _modFramework.TeleportPlayer(targetPlayer.entityId, existingRecord.Playfield,
                existingRecord.PosX, existingRecord.PosY, existingRecord.PosZ,
                existingRecord.RotX, existingRecord.RotY, existingRecord.RotZ);

            _dbManager.DeleteRecord(TetherportFormatter.FormatTetherportFileName(targetPlayer.steamId));
        }
    }
}
