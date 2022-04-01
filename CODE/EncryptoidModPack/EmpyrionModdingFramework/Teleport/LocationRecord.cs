using EmpyrionModdingFramework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework.Teleport
{
    public class LocationRecord
    {
        public LocationRecord() { } //For CsvHelper
        public string Name { get; set; }
        public char AdminYN { get; set; }
        public string Playfield { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
    }
}
