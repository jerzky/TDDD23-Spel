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

    }



    public static class ItemList
    {
        public const uint ITEM_LOCKPICK = 1;

        public static readonly Dictionary<uint, ItemInfo> AllItems = new Dictionary<uint, ItemInfo>
        {
            {
                ITEM_LOCKPICK, new ItemInfo
                {
                    UID = ITEM_LOCKPICK,
                    Name = "Lockpick",
                    IconPath = "Textures/drilltesticon",
                    IconIndex = 0
                }
             }
        };
    }

}
