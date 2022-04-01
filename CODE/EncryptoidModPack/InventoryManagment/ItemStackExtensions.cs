using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework
{
    public static class ItemStackExtensions
    {
        public static List<InventoryItem> ToInventoryItems(this ItemStack[] inv)
        {
            var ret = new List<InventoryItem>();
            if (inv == null) return ret;
            foreach (var item in inv)
            {
                ret.Add(item.ToInventoryItem());
            }
            return ret;
        }

        public static InventoryItem ToInventoryItem(this ItemStack itemstack)
        {
            return new InventoryItem(itemstack.id, itemstack.slotIdx, itemstack.count, itemstack.ammo, itemstack.decay);
        }

        public static ItemStack ToItemStack(this InventoryItem item)
        {
            return new ItemStack()
            {
                id = item.Id,
                slotIdx = (byte)item.SlotId,
                count = item.Count,
                ammo = item.Ammo,
                decay = item.Decay
            };
        }

        public static InventoryItem ClearItem(this InventoryItem item)
        { // SlotId is not cleared
            item.Id = 0;
            item.Ammo = 0;
            item.Count = 0;
            item.Decay = 0;

            return item;
        }

        public static List<InventoryItem> CleanSlotIds(this List<InventoryItem> items, bool isBag)
        {
            var arr = items.ToArray();

            for(var i = 0; i < arr.Count(); i++)
            {
                arr[i].SlotId = i;
            }
            return arr.ToList();
        }

        public static ItemStack[] CleanSlotIds(this ItemStack[] items, bool isBag)
        {
            for (byte i = 0; i < items.Count(); i++)
            {
                items[i].slotIdx = i;
            }
            return items;
        }
    }
}
