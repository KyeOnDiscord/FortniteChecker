namespace FortniteChecker;

internal class JsonBuilder
{

    public static object CreateJsonFile(List<CosmeticsDB.Datum> cosmetics, List<QueryProfile.Modal.PastSeason> stats, List<Banners.Datum> banners, string vbucks, string giftsSent, string giftsReceived, string createdTimestamp, string displayName, string lifeTimeWins, string epicID) => new
    {
        cosmetics = BuildCosmetics(cosmetics), // Assume these methods return serialized objects
        stats = stats,
        banners = banners,
        vbucks,
        giftsSent,
        giftsReceived,
        createdTimestamp,
        displayName,
        LifeTimeWins = lifeTimeWins,
        _id = epicID
    };

    public static List<object> BuildCosmetics(List<CosmeticsDB.Datum> cosmetics)
    {
        // Create a list to hold the structured cosmetics data
        List<object> cosmeticsList = new();
        // Add favorite items
        foreach (var item in cosmetics.Where(x => x.favourite).OrderBy(x => x.introduction.backendValue))
        {
            var cosmeticItem = new
            {
                name = item.name,
                id = item.id,
                type = item.type.backendValue,
                rarity = "mythic",
                variants = item.OwnedVariant.Select(variant => new
                {
                    Stage = variant.Stage,
                    Channel = variant.Channel
                }).ToList()
            };
            cosmeticsList.Add(cosmeticItem);
        }
        foreach (var item in cosmetics.Where(x => !x.favourite).OrderByDescending(x => x.rarity.backendIntValue).ThenBy(x => x.introduction.backendValue))
        {
            var cosmeticItem = new
            {
                Name = item.name,
                Id = item.id,
                Type = item.type.backendValue,
                Rarity = item.rarity.value,
                Variants = item.OwnedVariant.Select(variant => new
                {
                    Stage = variant.Stage,
                    Channel = variant.Channel
                }).ToList()
            };
            cosmeticsList.Add(cosmeticItem);
        }
        return cosmeticsList;
    }
}
