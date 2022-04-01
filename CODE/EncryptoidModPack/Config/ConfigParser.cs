using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpyrionModdingFramework;
using Newtonsoft.Json;

namespace Config
{
    public abstract class ConfigParser
    {
        private string _modPackDatabaseFolder;
        protected const string ConfigFileName = "config.json";

        protected abstract string ConfigFilePath { get; }

        public ConfigParser(string modPackDatabaseFolder)
        {
            _modPackDatabaseFolder = modPackDatabaseFolder;
        }

        public string ParseModConfig(string modName)
        {
            var path = ;
        }

        public void ParseConfig(string configPath)
        {
            var json = JsonConvert.DeserializeObject(File.ReadAllText(configPath));
        }
    }

    public class TetherportConfig: ConfigParser
    {
        protected override string ConfigFilePath { get; }

        public TetherportConfig(string modPackDatabaseFolder, string modName) : base(modPackDatabaseFolder)
        {
            ConfigFilePath = Path.Combine(modPackDatabaseFolder, modName, ConfigFileName);
        }

        public List<ChatCommand> ParseCommands(ChatCommand.ChatCommandHandler tetherportCallback, ChatCommand.ChatCommandHandler untethercallbakc)
        {
            var json = JsonConvert.DeserializeObjectFile.ReadAllText(ConfigFilePath));




        }

        public void ParseTetherportCommand(ChatCommand.ChatCommandHandler tetherportCallback)
        {

        }


        private class TetherportConfigJson
        {
            public string ActiveYN { get; set; }
            public string Level { get; set; }
            public string Commands { get; set; }
        }
    }


}
