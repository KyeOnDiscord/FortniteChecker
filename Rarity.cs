using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteChecker
{
    internal class Rarity
    {
        public enum EFortRarity
        {
            [Description("Uncommon")]
            Uncommon = 1, // Default

            [Description("Unattainable")]
            Impossible = 7,
            [Description("Unattainable")]
            Unattainable = 7,

            [Description("Exotic")]
            Exotic = 6,
            [Description("Exotic")]
            Transcendent = 6,

            [Description("Mythic")]
            Elegant = 5,
            [Description("Mythic")]
            Mythic = 5,

            [Description("Legendary")]
            Fine = 4,
            [Description("Legendary")]
            Legendary = 4,

            [Description("Epic")]
            Quality = 3,
            [Description("Epic")]
            Epic = 3,

            [Description("Rare")]
            Sturdy = 2,
            [Description("Rare")]
            Rare = 2,

            [Description("Common")]
            Handmade = 0,
            [Description("Common")]
            Common = 0
        }

        //Rarity Name to int
        public static int RarityToInt(string rarity)
        {
            switch (rarity)
            {
                case "Uncommon":
                    return 1;
                case "Rare":
                    return 2;
                case "Epic":
                    return 3;
                case "Legendary":
                    return 4;
                case "Mythic":
                    return 5;
                case "Exotic":
                    return 6;
                case "Unattainable":
                    return 7;
                default:
                    return 0;
            }
        }
    }
}
