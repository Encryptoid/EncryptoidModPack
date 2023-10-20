using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLocator
{
    public class EmpyrionLocator
    {
        private Action<string> _log;
        private const string ModFolderFileName = "modfolder.path";
        
        private readonly string _modName;
        private readonly string _modFolderPath;

        public EmpyrionLocator(Action<string> logFunc, string modName)
        {
            _log = logFunc;
            _modName = modName;
            _modFolderPath = GetModFolder();
        }

        public string GetConfigFile(string configFileName)
        {
            return Path.Combine(_modFolderPath, configFileName);
        }

        public string GetDatabaseFolder()
        {
            return Path.Combine(_modFolderPath, "Database");
        }

        private string GetModFolder()
        {
            var directionFile = Path.Combine(Directory.GetCurrentDirectory(), ModFolderFileName);

            if (!File.Exists(directionFile))
            {
                _log($"{_modName} cannot locate it's database path. Please place a {ModFolderFileName} file in the folder you are running the server from: {Directory.GetCurrentDirectory()}");
                return null;
            }
            var modPackFolderPath = Path.GetFullPath(File.ReadAllText(directionFile).Trim()); //Trim spaces and parse relative paths

            if (!Directory.Exists(modPackFolderPath)) //Check for ../Content/Mods/EncryptoidModPack directory
            {
                _log($"{_modName} cannot location it's database path, the full location it checked was: {modPackFolderPath}");
                return null;
            }

            return Path.Combine(modPackFolderPath, _modName);
        }
    }
}
