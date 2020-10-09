using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Tooltips
    {
        public const string DOOR_TOOLTIP = "Doors are interactable, simply press your interactable key({0}) to open it.";

        public const string SEARCHABLE_CONTAINER_TOOLTIP =
            "Searchable Containers are interactable, they come in different shapes and sizes, for instance a searchable container could be a desk or cabinet, simply press your interactable key({0}) to open it. If its locked, maybe you should consider buying a lockpick.";

        public const string STORE_TOOLTIP =
            "The store can be used to buy the items you need to complete your heists or cause mayhem, whatever, you prefer, simply press your interactable key({0}) to open it.";

        public const string DRILL_TOOLTIP = "The drill interactable is spawned when you use your drill item, you can leave it to work on its own, unlike the lockpick, if you want to cancel it and pick it up again, simply use your interact key({0}).";

        public const string CASH_REGISTER_TOOLTIP = "The Cash register is a interactable that can be found in a few different locations, such as the bar or gas station, but the most lucrative cashregister is found inside the lobby of the bank. Holding a weapon will trigger the armed robbery mode, if you're not holding a weapon you will try to rob it in stealth mode. Interact key: {0}";

        public const string CCTV_TOOLTIP = "The CCTV will alert guards if you walk into its vision, however, you will only be noticed if a guard is guarding the security station.";

        public const string SECURITY_STATION_TOOLTIP =
            "The security station controls the cameras, if there is no guard stationed at it you can freely move through the cameras' vision.";

        public const string GUARD_TEXT =
            "This is a guard, if you dont have a weapon use your takedown key({0}) while sneaking up behind him to knock him out.";
    }
