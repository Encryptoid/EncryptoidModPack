using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModLocator
{
    public class ConfigParser
    {
        private readonly string _modPackConfigFolder;

        public ConfigParser(string modPackConfigFolder)
        {
            _modPackConfigFolder = modPackConfigFolder;
        }

        public T ParseModConfig<T>(string modName)
        {
            var path = Path.Combine(_modPackConfigFolder, $"{modName}Config.json");

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
