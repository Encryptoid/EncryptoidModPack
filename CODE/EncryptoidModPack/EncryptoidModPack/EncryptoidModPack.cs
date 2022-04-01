using AdminTetherport;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using ModLocator;
using ModPackHelp;
using Retriever;
using Tetherport;

namespace EncryptoidModPack
{
    public class AdminModPack: EmpyrionModdingFrameworkBase
    {
        protected override void Initialize()
        {
            ModName = "EncryptoidModPack";
            CommandManager.CommandPrexix = "!";

            var helper = new ModPackHelper(this);
            CommandManager.CommandList.Add(new ChatCommand(ModPackHelper.HelpCommand, helper.ShowHelpWindow, PlayerPermission.Admin));

            //Setup Mods
            SetupTetherport();
            SetupAdminTetherport();
            SetupRetrieve();
        }

        private void SetupTetherport()
        {
            var modLocator = new EmpyrionLocator(Log, TetherportHandler.ModName);
            var tetherport = new TetherportHandler(new CsvManager(modLocator.GetDatabaseFolder()), this, Log);

            //All Players
            CommandManager.CommandList.Add(new ChatCommand(TetherportHandler.TetherportCommand, tetherport.ListPortals));
            CommandManager.CommandList.Add(new ChatCommand(TetherportHandler.TetherportShorthand, tetherport.ListPortals));

            //Admin Only
            CommandManager.CommandList.Add(new ChatCommand(TetherportHandler.PortalCreateCommand, tetherport.CreatePortal, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand(TetherportHandler.PortalAdminCommand, tetherport.PortalToggleAdmin, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand(TetherportHandler.PortalDeleteCommand, tetherport.DeletePortal, PlayerPermission.Admin));
        }

        private void SetupAdminTetherport()
        {
            var modLocator = new EmpyrionLocator(Log, AdminTetherportHandler.ModName);
            var adminTetherport = new AdminTetherportHandler(new CsvManager(modLocator.GetDatabaseFolder()), this, Log);

            CommandManager.CommandList.Add(new ChatCommand(AdminTetherportHandler.AdminTetherportCommand, adminTetherport.ShowOnlinePlayers, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand(AdminTetherportHandler.AdminTetherportShorthand, adminTetherport.ShowOnlinePlayers, PlayerPermission.Admin));
        }

        private void SetupRetrieve()
        {
            var modLocator = new EmpyrionLocator(Log, RetrieverHandler.ModName);
            var retriever = new RetrieverHandler(new CsvManager(modLocator.GetDatabaseFolder()), this, Log);

            CommandManager.CommandList.Add(new ChatCommand(RetrieverHandler.RetrieveCommand, retriever.DialogRetrievePlayer, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand(RetrieverHandler.RetrieveIssueCommand, retriever.DialogRetrievePlayer, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand(RetrieverHandler.ReturnCommand, retriever.DialogReturnPlayer, PlayerPermission.Admin));
        }
    }
}
