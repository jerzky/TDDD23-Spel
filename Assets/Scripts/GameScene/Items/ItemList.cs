using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Items
{
    public enum WeaponType { None, Ranged, Meele };
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
        public WeaponType WeaponType { get; set; } = WeaponType.None;

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
            WeaponType = WeaponType.Meele,
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
            WeaponType = WeaponType.Ranged,
            InventoryStackSize = 1
        };

        public static readonly ItemInfo ITEM_AMMO = new ItemInfo
        {
            UID = 4,
            Name = "Ammo",
            IconPath = "Textures/bullet",
            IconIndex = 0,
            AverageUseTime = 0f,
            WeaponType = WeaponType.None,
            InventoryStackSize = 64
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
            }
        };
    }

}
