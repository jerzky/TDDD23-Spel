using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Items
{
    public class ItemInfo
    {
        public uint UID { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public uint IconIndex { get; set; }
        public float AverageUseTime { get; set; }
    }



    public static class ItemList
    {
        public static readonly ItemInfo ITEM_LOCKPICK = new ItemInfo
        {
            UID = 1,
            Name = "Lockpick",
            IconPath = "Textures/drilltesticon",
            IconIndex = 0,
            AverageUseTime = 5f
        };

        public static readonly Dictionary<uint, ItemInfo> AllItems = new Dictionary<uint, ItemInfo>
        {
            {
                ITEM_LOCKPICK.UID, ITEM_LOCKPICK
            }
        };
    }

}
