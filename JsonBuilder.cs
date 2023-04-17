using System.Text;

namespace FortniteChecker;

internal class JsonBuilder
{

    public static string CreateJsonFile(List<CosmeticsDB.Datum> cosmetics, List<QueryProfile.Modal.PastSeason> stats, List<Banners.Datum> banners, string vbucks, string giftsSent, string giftsReceived, string createdTimestamp, string displayName, string LifeTimeWins, string EpicID)
    {
        StringBuilder sb = new StringBuilder();
        string cosmeticsList = BuildCosmetics(cosmetics);
        string statsList = BuildStats(stats);
        string bannerList = BuildBanners(banners);
        sb.Append("{\"cosmetics\":" + cosmeticsList);
        sb.Append(",\"stats\":" + statsList);
        sb.Append(",\"banners\":" + bannerList);
        sb.Append(",\"vbucks\":" + vbucks);
        sb.Append(",\"giftsSent\":" + giftsSent);
        sb.Append(",\"giftsReceived\":" + giftsReceived);
        sb.Append(",\"createdTimestamp\":" + createdTimestamp);
        sb.Append(",\"displayName\":\"" + displayName + "\"");
        sb.Append(",\"LifeTimeWins\":\"" + LifeTimeWins + "\"");
        sb.Append(",\"EpicID\":\"" + EpicID + "\"");
        sb.Append("}");
        return sb.ToString();
    }

    public static string BuildCosmetics(List<CosmeticsDB.Datum> cosmetics)
    {
        //Convert cosmetics to json array with strings
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        foreach (var item in cosmetics.Where(x => x.favourite == true).OrderBy(x => x.introduction.backendValue))
        {
            sb.Append("{");
            sb.Append($"\"name\":\"{item.name}\",");
            sb.Append($"\"id\":\"{item.id}\",");
            sb.Append($"\"type\":\"{item.type.backendValue}\",");
            sb.Append($"\"rarity\":\"mythic\"");
            if (item.OwnedVariant.Count > 0)
            {
                sb.Append($",\"variants\":[");
                foreach (var variant in item.OwnedVariant)
                {
                    sb.Append("{");
                    sb.Append($"\"Stage\":\"{variant.Stage}\",");
                    sb.Append($"\"Channel\":\"{variant.Channel}\"");
                    sb.Append("},");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }

            sb.Append("},");
        }

        foreach (var item in cosmetics.Where(x => x.favourite == false).OrderByDescending(x => x.rarity.backendIntValue).ThenBy(x => x.introduction.backendValue))
        {
            var cosmetic = cosmetics.Where(x => x.favourite == true).OrderBy(x => x.introduction.backendValue);
            sb.Append("{");
            sb.Append($"\"name\":\"{item.name}\",");
            sb.Append($"\"id\":\"{item.id}\",");
            sb.Append($"\"type\":\"{item.type.backendValue}\",");
            sb.Append($"\"rarity\":\"{item.rarity.value}\"");

            if (item.OwnedVariant.Count > 0)
            {
                sb.Append($",\"variants\":[");
                foreach (var variant in item.OwnedVariant)
                {
                    sb.Append("{");
                    sb.Append($"\"Stage\":\"{variant.Stage}\",");
                    sb.Append($"\"Channel\":\"{variant.Channel}\"");
                    sb.Append("},");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }

            sb.Append("},");
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
        return sb.ToString();
    }

    public static string BuildBanners(List<Banners.Datum> Banners)
    {
        //Convert cosmetics to json array with strings
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        foreach (var banner in Banners)
        {
            sb.Append("{");
            sb.Append($"\"name\":\"{banner.devName}\",");
            sb.Append($"\"id\":\"{banner.id}\",");
            sb.Append($"\"image\":\"{banner.images.smallIcon}\"");

            sb.Append("},");
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
        return sb.ToString();
    }


    public static string BuildStats(List<QueryProfile.Modal.PastSeason> stats)
    {
        //Convert cosmetics to json array with strings
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        for (int i = 0; i < stats.Count(); i++)
        {
            sb.Append("{");
            sb.Append($"\"seasonNumber\":\"{stats[i].seasonNumber}\",");
            sb.Append($"\"numWins\":\"{stats[i].numWins}\",");
            sb.Append($"\"seasonLevel\":\"{stats[i].seasonLevel}\",");
            sb.Append($"\"bookLevel\":\"{stats[i].bookLevel}\",");
            sb.Append($"\"purchasedVIP\":{stats[i].purchasedVIP.ToString().ToLower()}");

            sb.Append("},");
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
        return sb.ToString();
    }
}
