using System.Configuration;
using System.Collections.Specialized;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModLocator;

namespace Tetherport
{
    public partial class TetherportHandler
    {
        private readonly IDatabaseManager _dbManager;
        private readonly EmpyrionModdingFrameworkBase _modFramework;
        private readonly Action<string> _log;
        private readonly TetherportConfig _config;

        private const string PortalFileName = "portal.tether";
        private const string UntetherLocationName = "UNTETHER";

        public const string ModName = "Tetherport";
        public const string TetherportCommand = "tetherport";
        public const string TetherportShorthand = "ttp";
        public const string TetherportConfig = "tetherport_config.yaml";

        public TetherportHandler(IDatabaseManager dbManager, EmpyrionModdingFrameworkBase modFramework,
            Action<string> logFunc, TetherportConfig config)
        {
            _dbManager = dbManager;
            _modFramework = modFramework;
            _log = logFunc;
            _config = config;
        }

        public async Task ListPortals(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);

            await _modFramework.IsSeatedInBa(player);

            var unfilteredRecords = _dbManager.LoadRecords<PortalRecord>(PortalFileName) ?? new List<PortalRecord>();
            var records = new List<PortalRecord>();
            foreach(var record in unfilteredRecords)
            {
                // Do not add Ship=N records if the player is sitting, unless they are an admin
                if (!player.IsAdmin() && record.ShipYN.N() && player.IsSeated())
                    continue;

                records.Add(record);
            }

            records = AddUntetherRecord(records, player);

            var locList = TetherportFormatter.FormatLocationList(records,
                _modFramework.IsAdmin(player.permission),
                GetTtpString(player.IsSeated()), TetherportAdminCommands);

            if (!player.IsSeated())
            {
                _modFramework.ShowLinkedTextDialog(player.entityId, locList,
                    "Tetherport Information!", DialogTetherportPlayer);
            }
            else
            {
                _modFramework.ShowLinkedTextDialog(player.entityId, TetherportFormatter.FormatLocationList(records,
                        _modFramework.IsAdmin(player.permission),
                        GetTtpString(player.IsSeated()), TetherportAdminCommands),
                    "Tetherport Information!", DialogConfirmTetherportPlayer, "Confirm");
            }
        }

        private string GetTtpString(bool isSeated)
        {
            var str = _config.TtpString;

            str += isSeated ? "<color=\"red\"> You are seated so some portals may not be available to you.</color>" : "";

            return str;
        }
        
        private List<PortalRecord> AddUntetherRecord(List<PortalRecord> records, PlayerInfo player)
        {
            var untetherRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            if (untetherRecord != null)
            {
                var untetherLocation = new PortalRecord()
                {
                    Name = UntetherLocationName,
                    AdminYN = 'N',
                    ShipYN = 'N',
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

            return records;
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

            await _modFramework.MessagePlayer(player.entityId, _config.SuccessfulUntether, 5);
        }

        private async void DialogConfirmTetherportPlayer(int buttonIdx, string linkId, string inputContent,
            int playerId, int customValue)
        {
            var player = await _modFramework.QueryPlayerInfo(playerId);

            var warningStr = _config.SeatedWarning;

            var portalRecord = _dbManager.LoadRecords<PortalRecord>(PortalFileName)
                .Where(e => string.Equals(e.Name, linkId)).ToList();

            if (portalRecord.Count != 0)
            {
                _modFramework.ShowLinkedTextDialog(playerId, TetherportFormatter.FormatLocationList(portalRecord, player.IsAdmin(),
                        warningStr),
                    "Confirm Seated Tetherport", DialogTetherportPlayer);
            }

            if (string.Equals(linkId, UntetherLocationName))
            {
                _modFramework.ShowLinkedTextDialog(playerId, TetherportFormatter.FormatLocationList(AddUntetherRecord(new List<PortalRecord>(), player), player.IsAdmin(),
                        warningStr),
                    "Confirm Seated Untether", DialogTetherportPlayer);
            }
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

            var isSitting = player.IsSeated();
            _log("Seated status1: " + isSitting);

            //This is intentionally not awaited as there is a task delay after
            _modFramework.MessagePlayer(player.entityId, $"Initiating Tetherport. {_config.TetherportDelay} seconds to launch.", _config.TetherportDelay);
            await Task.Delay(new TimeSpan(0, 0, _config.TetherportDelay));

            //Requery player info
            player = await _modFramework.QueryPlayerInfo(playerId);
            var stillSeated = player.IsSeated();
            _log("Seated status2: " + stillSeated);

            if (isSitting != stillSeated)
            {
                await _modFramework.MessagePlayer(player.entityId,
                    "You are in a different seated state than when your tetherport began. Cancelling.", 5,
                    MessagerPriority.Red);
                return;
            }

            var existingRecord = _dbManager.LoadRecords<PlayerLocationRecord>(TetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            //Only save new tether record if one does not exist
            if (existingRecord == null)
            {
                _log($"Saving new record tetherport record for EntityId: {playerId}");
                _dbManager.SaveRecord(TetherportFormatter.FormatTetherportFileName(player.steamId), player.ToPlayerLocationRecord(), true);
            }
 
            var portals = _dbManager.LoadRecords<PortalRecord>(PortalFileName);
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
