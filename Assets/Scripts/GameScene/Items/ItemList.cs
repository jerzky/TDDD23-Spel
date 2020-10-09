using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Items
{
    public enum ItemType { None, Weapon, MeleeWeapon, Usable };
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
        public uint SellPrice { get; set; }
        public uint BuyPrice { get; set; }
        public uint PurchaseAmount { get; set; } = 1;

        public float SoundRadius { get; set; } = 25f;
        public string Tooltip { get; set; }

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
            InventoryStackSize = 10,
            SellPrice = 10,
            BuyPrice = 100,
            PurchaseAmount = 1,
            SoundRadius = 2f,
            Tooltip = "The lockpick can be used on locked interactables, such as doors or cabinets, by pressing the use item key({0})."
        };

        public static readonly ItemInfo ITEM_SLEDGEHAMMER = new ItemInfo
        {
            UID = 2,
            Name = "SledgeHammer",
            IconPath = "Textures/sledgeHammer",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 25,
            BreakableDamage = 10,
            ItemType = ItemType.MeleeWeapon,
            InventoryStackSize = 1,
            SellPrice = 50,
            BuyPrice = 200,
            PurchaseAmount = 1,
            SoundRadius = 5f,
            Tooltip = "This can be used to break walls, so you can enter a building from a different direction. Unless you want to simply hit the guards with it. It is used as a weapon by pressing the Shoot key({0})."
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
            InventoryStackSize = 1,
            SellPrice = 500,
            BuyPrice = 1000,
            PurchaseAmount = 1,
            SoundRadius = 30f,
            Tooltip = "Weapons can be used to control civilians or fight guards and police, simply point the gun at a civilian that has noticed you to make the civilian freeze."
        };

        public static readonly ItemInfo ITEM_PISTOL_AMMO = new ItemInfo
        {
            UID = 4,
            Name = "Pistol_Ammo",
            IconPath = "Textures/bullet",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.None,
            InventoryStackSize = 640,
            SellPrice = 100,
            BuyPrice = 1000,
            PurchaseAmount = 16
        };

        public static readonly ItemInfo ITEM_EXPLOSIVE_REMOTE = new ItemInfo
        {
            UID = 5,
            Name = "REMOTE C4",
            IconPath = "Textures/c4",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.Usable,
            InventoryStackSize = 64,
            BreakableDamage = 150,
            PrefabPath = "Assets/Prefabs/Explosive",
            SellPrice = 1000,
            BuyPrice = 5000,
            PurchaseAmount = 1,
            SoundRadius = 100f
        };

        public static readonly ItemInfo ITEM_EXPLOSIVE_TIMED = new ItemInfo
        {
            UID = 6,
            Name = "TIMED C4",
            IconPath = "Textures/c4",
            IconIndex = 0,
            AverageUseTime = 0f,
            ItemType = ItemType.Usable,
            InventoryStackSize = 64,
            BreakableDamage = 150,
            PrefabPath = "Assets/Prefabs/Explosive",
            SellPrice = 1000,
            BuyPrice = 5000,
            PurchaseAmount = 1,
            SoundRadius = 100f,
            Tooltip = "The C4 is a highpowered explosive which can break down pretty much anything on the map. Use it by pressing the use item key({0})."
        };

        public static readonly ItemInfo ITEM_SILENCED_PISTOL = new ItemInfo
        {
            UID = 7,
            Name = "Silenced Pistol",
            IconPath = "Textures/silencedpistol",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 50,
            BreakableDamage = 0,
            ItemType = ItemType.Weapon,
            InventoryStackSize = 1,
            SellPrice = 500,
            BuyPrice = 2000,
            PurchaseAmount = 1,
            SoundRadius = 10f,
            Tooltip = ITEM_PISTOL.Tooltip
        };

        public static readonly ItemInfo ITEM_AK47 = new ItemInfo
        {
            UID = 8,
            Name = "AK47",
            IconPath = "Textures/AK47",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 75,
            BreakableDamage = 0,
            ItemType = ItemType.Weapon,
            InventoryStackSize = 1,
            SellPrice = 1000,
            BuyPrice = 6000,
            PurchaseAmount = 1,
            SoundRadius = 40f,
            Tooltip = ITEM_PISTOL.Tooltip
        };

        public static readonly ItemInfo ITEM_DRILL = new ItemInfo
        {
            UID = 9,
            Name = "Drill",
            IconPath = "Textures/drilltesticon",
            IconIndex = 0,
            AverageUseTime = 15f,
            ItemType = ItemType.Usable,
            InventoryStackSize = 1,
            SellPrice = 2000,
            BuyPrice = 5000,
            PurchaseAmount = 1,
            SoundRadius = 10f,
            Tooltip = "The drill is used to open locked interactables, similar to the lockpick, however, its main use is to open the vault doors in the bank. Use it by pressing the use item key({0})."
        };

        public static readonly ItemInfo ITEM_MAC10 = new ItemInfo
        {
            UID = 12,
            Name = "MAC-10",
            IconPath = "NoSpriteAtlasTextures/mac_10",
            IconIndex = 0,
            AverageUseTime = 0f,
            HumanDamage = 50,
            BreakableDamage = 0,
            ItemType = ItemType.Weapon,
            InventoryStackSize = 1,
            SellPrice = 1000,
            BuyPrice = 6000,
            PurchaseAmount = 1,
            SoundRadius = 30f,
            Tooltip = ITEM_PISTOL.Tooltip
        };

        public static readonly ItemInfo ITEM_ZIPTIES = new ItemInfo
        {
            UID = 13,
            Name = "ZIPTIES",
            IconPath = "Textures/x64spritesheet",
            IconIndex = 66,
            AverageUseTime = 3f,
            HumanDamage = 0,
            BreakableDamage = 0,
            ItemType = ItemType.Usable,
            InventoryStackSize = 64,
            SellPrice = 0,
            BuyPrice = 100,
            PurchaseAmount = 5,
            SoundRadius = 2f
        };

        public static readonly ItemInfo ITEM_CELLPHONE_JAMMER = new ItemInfo
        {
            UID = 14,
            Name = "CellPhoneJammer",
            IconPath = "NoSpriteAtlasTextures/cellphonejammer",
            IconIndex = 0, 
            AverageUseTime = 1f,
            HumanDamage = 0,
            BreakableDamage = 0,
            ItemType = ItemType.Usable,
            InventoryStackSize = 5,
            SellPrice = 3000,
            BuyPrice = 6000,
            PurchaseAmount = 1,
            SoundRadius = 0f
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
                ITEM_PISTOL_AMMO.UID, ITEM_PISTOL_AMMO
            },
            /*{
                ITEM_EXPLOSIVE_REMOTE.UID, ITEM_EXPLOSIVE_REMOTE
            },*/
            {
                ITEM_EXPLOSIVE_TIMED.UID, ITEM_EXPLOSIVE_TIMED
            },
            {
                ITEM_SILENCED_PISTOL.UID, ITEM_SILENCED_PISTOL
            },
            {
                ITEM_AK47.UID, ITEM_AK47
            },
            {
                ITEM_DRILL.UID, ITEM_DRILL
            },
            {
                ITEM_MAC10.UID, ITEM_MAC10
            },
            {
                ITEM_ZIPTIES.UID, ITEM_ZIPTIES
            },
            {
                ITEM_CELLPHONE_JAMMER.UID, ITEM_CELLPHONE_JAMMER
            }

        };
    }

}
