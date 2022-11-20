namespace FortniteChecker;

internal class Rarity
{
    //Rarity Name to int
    public static int RarityToInt(string rarity)
    {
        switch (rarity.ToLower())
        {

            case "uncommon": return 1;
            case "rare": return 2;
            case "epic": return 3;
            case "marvel": return 4;
            case "starwars": return 5;
            case "icon": return 6;
            case "dc": return 7;
            case "gaminglegends": return 8;
            case "dark": return 9;
            case "frozen": return 10;
            case "lava": return 11;
            case "shadow": return 12;
            case "slurp": return 13;
            case "legendary": return 14;
            case "mythic": return 15;
            default:
            case "common": return 0;
        }

    }
}