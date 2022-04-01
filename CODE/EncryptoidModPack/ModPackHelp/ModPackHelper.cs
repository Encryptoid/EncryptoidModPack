using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using EmpyrionModdingFramework;

namespace ModPackHelp
{
    public class ModPackHelper
    {
        private readonly EmpyrionModdingFrameworkBase _modFramework;

        public const string HelpCommand = "modpack";

        public ModPackHelper(EmpyrionModdingFrameworkBase modFramework)
        {
            _modFramework = modFramework;
        }

        public async Task ShowHelpWindow(MessageData messageData)
        {
            var adminPlayer = await _modFramework.QueryPlayerInfo(messageData.SenderEntityId);
            if (adminPlayer == null) return;

            _modFramework.ShowLinkedTextDialog(adminPlayer.entityId, ModPackHelpFormatter.FormatHelpMessage(), "Encryptoid Mod Pack Help",
                (i, s, arg3, arg4, arg5) => { }, "Close");
        }
    }
}
