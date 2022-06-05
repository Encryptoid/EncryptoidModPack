using AdminTetherport;
using Retriever;
using Tetherport;

namespace ModPackHelp
{
    public static class ModPackHelpFormatter
    {
        public static string FormatHelpMessage()
        {
            var uiString = "";

            uiString += Header("Encryptoid Mod Pack Help");
            uiString += $"Here you will find a list of available commands and what they do. Each mod is independent. Tetherport, AdminTetherport and Retrieve will not share tethers.\n\n";


            uiString += Header($"Tetherport");
            uiString +=
                $"This mod allows you to teleport to a tetherport location(portal), creating a tether at your existing location that you can return to.\n\n";
            uiString += $"!{TetherportHandler.TetherportCommand} or !{TetherportHandler.TetherportShorthand}\n";
            uiString += "This will launch a UI showing all portals. If you have an existing tether, then it will show up at the top." +
                        " Using untether will remove it. Using another tether will not change your untether point.\n\n";

            uiString +=
                $"!{TetherportHandler.PortalCreateCommand}\nThis will launch a UI with a text box that will allow you to type in a portal name, and create it. " +
                $"Created portals start with AdminYN=Y.\n\n";

            uiString +=
                $"!{TetherportHandler.PortalAdminCommand}\nThis will launch a UI showing all portals. " +
                $"Clicking on one will toggle the AdminYN value from Y -> N or N -> Y, allowing the portals to be only visible to everyone or just admins.\n\n";

            uiString +=
                $"!{TetherportHandler.PortalDeleteCommand}\nThis will launch a UI showing portals." +
                $" Clicking one will take you to a confirm window, where you can click again to delete it.\n\n";

            uiString += Header("AdminTetherport");
            uiString +=
                $"!{AdminTetherportHandler.AdminTetherportCommand} or !{AdminTetherportHandler.AdminTetherportShorthand}\n";
            uiString +=
                "This will launch a UI showing all online players. If you have an existing tether it will show up at the top. " +
                "Selecting one of the players will create a tether for you, and teleport you to them. " +
                "Using another admin tether will overwrite your existing one. So untether before use if you want to save your original location.\n\n";

            uiString += $"!{AdminTetherportHandler.AdminTetherportOffsetCommand}\nThis will work the same as the other commands but will teleport you to an offset location.\n\n";

            uiString += Header("Retriever");
            uiString += $"!{RetrieverHandler.RetrieveCommand}\n";
            uiString +=
                "This command will launch a UI showing all online players. Selecting one of the players will teleport the player to your location and create a tether for them.\n\n";

            uiString += $"!{RetrieverHandler.RetrieveIssueCommand}\n";
            uiString += $"The same as !{RetrieverHandler.RetrieveCommand} but will not create a tether for the player. Useful for solving tickets where people are stuck.\n\n";

            uiString += $"!{RetrieverHandler.ReturnCommand}\n";
            uiString +=
                "This command will launch a UI showing all existing Retriever tethers. Using the tether will clear it so a large list is not built up.\n\n";

            return uiString;
        }

        private static string Header(string header)
        {
            return $"<line-height=250%>=={header}==</line-height>\n";
        }
    }
}