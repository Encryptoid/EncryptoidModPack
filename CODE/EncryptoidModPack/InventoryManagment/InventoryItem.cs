using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public int Count { get; set; }
        public int Ammo { get; set; }
        public int Decay { get; set; }

        public InventoryItem(int id, int slotId, int count, int ammo, int decay)
        {
            Id = id;
            SlotId = slotId;
            Count = count;
            Ammo = ammo;
            Decay = decay;
        }

        public InventoryItem() { } //For CsvHelper
    }
}
