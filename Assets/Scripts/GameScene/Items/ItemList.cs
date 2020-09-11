using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Items
{
    public enum ItemType { None, Weapon, Usable };
    public class ItemInfo
    {
        public uint UID { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public uint IconIndex { get; set; }
        public uint InventoryStackSize { get; set; } = 64;
        public float AverageUseTime { get; set; }
        public int HumanDamage { get; set; } = 0;
        public int BreakableDamage { get; set; } = 0;
        public ItemType ItemType { get; set; } = ItemType.None;
        public string PrefabPath { get; set; }


    }



    public static class ItemList
    {
        public static readonly ItemInfo ITEM_LOCKPICK = new ItemInfo
        {
            UID = 1,
            Name = "Lockpick",
            IconPath = "Textures/drilltesticon",
            IconIndex = 0,
            AverageUseTime = 5f,
            InventoryStackSize = 10
        };

        public static readonly ItemInfo ITEM_SLEDGEHAMMER = new ItemInfo
        {
            UID = 2,
            Name = "SledgeHammer",
            IconPath = "Textures/sledgeHammer",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 10,
            BreakableDamage = 10,
            ItemType = ItemType.Weapon,
            InventoryStackSize = 1
        };

        public static readonly ItemInfo ITEM_PISTOL = new ItemInfo
        {
            UID = 3,
            Name = "Pistol",
            IconPath = "Textures/pistol",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 50,
            BreakableDamage = 0,
            ItemType = ItemType.Weapon,
            InventoryStackSize = 1
        };

        public static readonly ItemInfo ITEM_AMMO = new ItemInfo
        {
            UID = 4,
            Name = "Ammo",
            IconPath = "Textures/bullet",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.None,
            InventoryStackSize = 64
        };

        public static readonly ItemInfo ITEM_EXPLOSIVE_REMOTE = new ItemInfo
        {
            UID = 5,
            Name = "Explosive",
            IconPath = "Textures/c4",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.Usable,
            InventoryStackSize = 64,
            PrefabPath = "Assets/Prefabs/Explosive"
        };

        public static readonly ItemInfo ITEM_EXPLOSIVE_TIMED = new ItemInfo
        {
            UID = 6,
            Name = "Explosive",
            IconPath = "Textures/c4",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.Usable,
            InventoryStackSize = 64,
            PrefabPath = "Assets/Prefabs/Explosive"
        };

        public static readonly Dictionary<uint, ItemInfo> AllItems = new Dictionary<uint, ItemInfo>
        {
            {
                ITEM_LOCKPICK.UID, ITEM_LOCKPICK
            },
            {
                ITEM_SLEDGEHAMMER.UID, ITEM_SLEDGEHAMMER
            },
            {
                ITEM_PISTOL.UID, ITEM_PISTOL
            },
            {
                ITEM_AMMO.UID, ITEM_AMMO
            },
            {
                ITEM_EXPLOSIVE_REMOTE.UID, ITEM_EXPLOSIVE_REMOTE
            },
            {
                ITEM_EXPLOSIVE_TIMED.UID, ITEM_EXPLOSIVE_TIMED
            }
        };
    }

}
