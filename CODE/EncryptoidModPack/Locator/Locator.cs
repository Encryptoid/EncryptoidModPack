using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Locator
{
    public abstract class Locator
    {
        protected abstract string ComponentName { get; }

        protected string ModPackDatabaseFolder;
        protected Locator(string modPackDatabaseFolder)
        {
            ModPackDatabaseFolder = modPackDatabaseFolder;
        }

        protected string GetComponentFolderPath(string folderName)
        {
            return Path.Combine(ModPackDatabaseFolder, ComponentName, folderName);
        }
    }

    public class TetherportLocator: Locator
    {
        public TetherportLocator(string modPackDatabaseFolder) : base(modPackDatabaseFolder)
        {
        }

        protected override string ComponentName => "Tetherport";

    }

    public class ConfigParser
    {

    }
}
