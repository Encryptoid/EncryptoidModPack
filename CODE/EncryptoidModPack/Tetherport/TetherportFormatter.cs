using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpyrionModdingFramework.Teleport;

namespace Tetherport
{
    public static class TetherportFormatter
    {
        public static string FormatLocationList(List<PortalRecord> locations, bool isAdmin, string startingText, List<string> commands = null)
        {
            var uiString = startingText + "\n\n";

            foreach (var location in locations)
            {
                if (!isAdmin && location.AdminYN == 'N')
                {
                    uiString += $"{FormatLocation(location)}\n";
                }

                if (isAdmin)
                {
                    uiString += $"{AdminFormatLocation(location)}\n";
                }
            }

            uiString += "\n\n";

            if (isAdmin && commands != null)
            {
                uiString += $"The following commands are made available to admins:\n";
                foreach (var command in commands)
                {
                    uiString += $"{FormatCommand(command)}\n";
                }
            }

            return uiString;
        }

        public static string AdminFormatLocation(PortalRecord location)
        {
            return $"<link=\"{location.Name}\"><indent=15%><line-height=150%>{location.Name} | {location.Playfield} | AdminYN={location.AdminYN} | ShipYN={location.ShipYN}</line-height></indent></link>";
        }

        private static string FormatLocation(PortalRecord location)
        {
            return $"<link=\"{location.Name}\"><indent=15%><line-height=150%>{location.Name} | {location.Playfield}</line-height></indent></link>";
        }

        private static string FormatCommand(string command)
        {
            return $"Command: !{command}";
        }

        public static string FormatTetherportFileName(string steamId)
        {
            return Path.Combine("Tethers", $"{steamId}.tether");
        }
    }
}
