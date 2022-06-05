using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;

namespace BackpackRestore
{
    public class BackpackRestore
    {

        private readonly IDatabaseManager _dbManager;
        private readonly EmpyrionModdingFrameworkBase _modFramework;
        private readonly Action<string> _log;

        public const string ModName = "BackpackRestore";
        public const string RestoreCommand = "restore-back";
        public const int UntetherLinkId = -99;
    }
}
