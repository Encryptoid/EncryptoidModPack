﻿using Eleon;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Teleport;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tetherport
{
    public partial class TetherportHandler
    {
        public readonly List<string> TetherportAdminCommands = new List<string>
        {
            PortalCreateCommand,
            PortalAdminCommand,
            PortalShipCommand,
            PortalDeleteCommand
        };
        public const string PortalCreateCommand = "portal-create";
        public const string PortalDeleteCommand = "portal-delete";
        public const string PortalAdminCommand = "portal-admin";
        public const string PortalShipCommand = "portal-ship";

        public async Task CreatePortal(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);

            _modFramework.ShowTextBoxDialog(player.entityId, "Please input a portal name below and a portal will be created at your current location.",
                "Create Portal!", "Create", DialogCreatePortal, "Type portal name here");
        }

        private async void DialogCreatePortal(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            var player = await _modFramework.QueryPlayerInfo(playerId);
            var portalName = inputContent.Trim();

            if (string.IsNullOrWhiteSpace(portalName))
            {
                await _modFramework.MessagePlayer(playerId, "Could not create portal. Portal name was blank.", 5,
                    MessagerPriority.Red);
                return;
            }

            var newLocation = new PortalRecord()
            {
                Name = portalName,
                AdminYN = 'Y', //Portals are admin only by default
                ShipYN = 'N',
                Playfield = player.playfield,
                PosX = player.pos.x,
                PosY = player.pos.y,
                PosZ = player.pos.z,
                RotX = player.rot.x,
                RotY = player.rot.y,
                RotZ = player.rot.z
            };

            _dbManager.SaveRecord(PortalFileName, newLocation, false);

            await _modFramework.MessagePlayer(player.entityId, $"Successfully created portal: {portalName}", 5);
        }

        public async Task PortalToggleAdmin(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            var records = _dbManager.LoadRecords<PortalRecord>(PortalFileName);

            var locList = TetherportFormatter.FormatLocationList(records, true,
                "Please select a Portal below to toggle it's Admin status",
                TetherportAdminCommands);

            _modFramework.ShowLinkedTextDialog(player.entityId, locList, 
                "Toggle Admin Visibility", DialogToggleAdmin);
        }

        private async void DialogToggleAdmin(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            var success = false;
            var existingRecords = _dbManager.LoadRecords<PortalRecord>(PortalFileName);
            var newRecords = new List<PortalRecord>();
            var oldValue = ' ';

            foreach (var portal in existingRecords)
            {
                if (string.Equals(portal.Name, linkId))
                {
                    oldValue = portal.AdminYN;
                    portal.AdminYN = InvertYN(portal.AdminYN);
                    success = true;
                }
                newRecords.Add(portal);
            }

            if (success)
            {
                _dbManager.SaveRecords(PortalFileName, newRecords, true);
                await _modFramework.MessagePlayer(playerId, $"Successfully set Admin privileges for portal \"{linkId}\" from \'{oldValue}\' to '{InvertYN(oldValue)}'",
                    5, MessagerPriority.Yellow);
            }
            else
            {
                await _modFramework.MessagePlayer(playerId, $"Failed to changed portal Admin status. Attempted Name Lookup: \"{linkId}\"",
                    5, MessagerPriority.Red);
            }
        }

        public async Task PortalToggleShip(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            var records = _dbManager.LoadRecords<PortalRecord>(PortalFileName);

            _modFramework.ShowLinkedTextDialog(player.entityId, TetherportFormatter.FormatLocationList(records, true,
                "Please select a Portal below to toggle whether ships are allowed", TetherportAdminCommands),
                "Toggle Ship Visibility", DialogToggleShip);
        }

        private async void DialogToggleShip(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            var success = false;
            var existingRecords = _dbManager.LoadRecords<PortalRecord>(PortalFileName);
            var newRecords = new List<PortalRecord>();
            var oldValue = ' ';

            foreach (var portal in existingRecords)
            {
                if (string.Equals(portal.Name, linkId))
                {
                    oldValue = portal.ShipYN;
                    portal.ShipYN = InvertYN(portal.ShipYN);
                    success = true;
                }
                newRecords.Add(portal);
            }

            if (success)
            {
                _dbManager.SaveRecords(PortalFileName, newRecords, true);
                await _modFramework.MessagePlayer(playerId, $"Successfully set Ship allowance for portal \"{linkId}\" from \'{oldValue}\' to '{InvertYN(oldValue)}'",
                    5, MessagerPriority.Yellow);
            }
            else
            {
                await _modFramework.MessagePlayer(playerId, $"Failed to changed portal Ship status. Attempted Name Lookup: \"{linkId}\"",
                    5, MessagerPriority.Red);
            }
        }

        public async Task DeletePortal(MessageData messageData)
        {
            var player = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            var records = _dbManager.LoadRecords<PortalRecord>(PortalFileName);

            _modFramework.ShowLinkedTextDialog(player.entityId, TetherportFormatter.FormatLocationList(records, true, 
                "Please select one of the portals below to delete it.", 
                new List<string> { PortalCreateCommand, PortalAdminCommand, PortalDeleteCommand }), "Delete Portal", ConfirmDeletePortalDialog);
        }

        private void ConfirmDeletePortalDialog(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            var portalRecord = _dbManager.LoadRecords<PortalRecord>(PortalFileName)
                .Where(e => string.Equals(e.Name, linkId)).ToList();

            if (portalRecord.Count != 0)
            {
                _modFramework.ShowLinkedTextDialog(playerId, TetherportFormatter.FormatLocationList(portalRecord, true,
                        "Are you sure you want to delete this portal? Click on it again to delete it permanently."),
                    "Confirm delete", DeletePortalDialog);
            }
        }

        private async void DeletePortalDialog(int buttonIdx, string portalNameLinkId, string inputContent, int playerId,
            int customValue)
        {
            var player = await _modFramework.QueryPlayerInfo(playerId);

            var success = false;
            var existingRecords = _dbManager.LoadRecords<PortalRecord>(PortalFileName);
            var newRecords = new List<PortalRecord>();

            foreach (var portal in existingRecords)
            {
                if (portal.Name == portalNameLinkId)
                {
                    success = true;
                }
                else
                {
                    newRecords.Add(portal);
                }
            }

            if (success)
            {
                _dbManager.SaveRecords(PortalFileName, newRecords, true);
                await _modFramework.MessagePlayer(player.entityId, $"Successfully deleted Portal: {portalNameLinkId}", 5, MessagerPriority.Red);
            }
            else
            {
                await _modFramework.MessagePlayer(playerId, $"Did not delete Portal: {portalNameLinkId}", 5);
            }
        }

        protected char InvertYN(char yn)
        {
            return yn == 'Y' ? 'N' : 'Y';
        }
    }
}
