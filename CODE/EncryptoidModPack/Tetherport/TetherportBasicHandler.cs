using Eleon;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tetherport
{
    public partial class TetherportHandler
    {
        private readonly IDatabaseManager _dbManager;
        private readonly EmpyrionModdingFrameworkBase _modFramework;
        private readonly Action<string> _log;

        private const string PortalFileName = "portal.tether";
        private const string UntetherLocationName = "UNTETHER";

        public const string ModName = "Tetherport";
        public const string TetherportCommand = "tetherport";
        public const string TetherportShorthand = "ttp";

        public TetherportHandler(IDatabaseManager dbManager, EmpyrionModdingFrameworkBase modFramework, Action<string> logFunc)
        {
            _dbManager = dbManager;
            _modFramework = modFramework;
            _log = logFunc;
        }

        public async Task ListPortals(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            var records = _dbManager.LoadRecords<LocationRecord>(PortalFileName) ?? new List<LocationRecord>();

            var untetherRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            if (untetherRecord != null)
            {
                var untetherLocation = new LocationRecord()
                {
                    Name = UntetherLocationName,
                    AdminYN = 'N',
                    Playfield = untetherRecord.Playfield,
                    PosX = untetherRecord.PosX,
                    PosY = untetherRecord.PosY,
                    PosZ = untetherRecord.PosZ,
                    RotX = untetherRecord.RotX,
                    RotY = untetherRecord.RotY,
                    RotZ = untetherRecord.RotZ,
                };
                records.Insert(0, untetherLocation);
            }

            _modFramework.ShowLinkedTextDialog(player.entityId, TetherportFormatter.FormatLocationList(records, _modFramework.IsAdmin(player.permission),
                    $"Click on one of the below locations to Tetherport there! If you have an Untether, it will not be removed until you use it!", new List<string> { PortalCreateCommand, PortalAdminCommand, PortalDeleteCommand }), 
                "Tetherport Information!", DialogTetherportPlayer);
        }

        private async Task Untether(int entityId)
        {
            var player = await _modFramework.QueryPlayerInfo(entityId);

            var existingRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();
            if (existingRecord == null)
            {
                _log($"Entity {player.entityId}/{player.playerName} requested Untether but no tether was found @ " + TetherportFormatter.FormatTetherportFileName(player.steamId));
                await _modFramework.MessagePlayer(player.entityId, $"No untether was found.", 5, MessagerPriority.Red);
                return;
            }

            await _modFramework.TeleportPlayer(player.entityId, existingRecord.Playfield,
                existingRecord.PosX, existingRecord.PosY, existingRecord.PosZ,
                existingRecord.RotX, existingRecord.RotY, existingRecord.RotZ);

            _dbManager.DeleteRecord(TetherportFormatter.FormatTetherportFileName(player.steamId));
            _log($"Deleted Tetherport record for SteamId: {player.steamId}, CurrentEntityId: {player.entityId}");
        }

        private async void DialogTetherportPlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            _log($"Entity Id: {playerId} began tetherport to {linkId}");
            if (string.IsNullOrWhiteSpace(linkId))
                return;

            var player = await _modFramework.QueryPlayerInfo(playerId);

            if (string.Equals(linkId, UntetherLocationName))
            {
                await Untether(player.entityId);
                return;
            }

            var existingRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            //Only save new tether record if one does not exist
            if (existingRecord == null)
            {
                _log($"Saving new record tetherport record for EntityId: {playerId}");
                _dbManager.SaveRecord(TetherportFormatter.FormatTetherportFileName(player.steamId), player.ToPlayerLocationRecord(), true);
            }
 
            var portals = _dbManager.LoadRecords<LocationRecord>(PortalFileName);
            if (portals == null)
            {
                await _modFramework.MessagePlayer(player.entityId, $"Could not find Tetherport Locations.", 10, MessagerPriority.Red);
                return;
            }

            // Find matching record and teleport player
            foreach (var portal in portals)
            {
                if (string.Equals(portal.Name, linkId))
                {
                    //Teleport and inform player
                    await _modFramework.TeleportPlayer(player.entityId, portal.Playfield, portal.PosX, portal.PosY, portal.PosZ, portal.RotX, portal.RotY, portal.RotZ);
                    await _modFramework.MessagePlayer(player.entityId, $"Created Tetherport tether! Welcome to {portal.Name}!", 10);
                    return;
                }
            }
        }
    }
}
