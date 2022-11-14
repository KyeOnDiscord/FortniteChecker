using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FortniteChecker
{
    //https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(CosmeticsDB.CosmeticsDBRoot))]
    [JsonSerializable(typeof(Auth.Modal.AuthRoot))]
    [JsonSerializable(typeof(QueryProfile.Modal.QueryProfileRoot))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }

    internal class Program
    {
        //Compile with native with dotnet publish -r win-x64 -c Release

        static void Main(string[] args)
        {

            //Create fortnitePCGameClient
            var fortnitePCGameClient = new AuthClient("ec684b8c687f479fadea3cb2ad83f5c6", "e1f31c211f28413186262d37a13fc84d");
            CosmeticsDB.CosmeticsDBRoot cosmetics = JsonSerializer.Deserialize(File.ReadAllText("br.json"), SourceGenerationContext.Default.CosmeticsDBRoot);

            //Open web link
            string url = $"https://www.epicgames.com/id/api/redirect?clientId={fortnitePCGameClient.ClientID}&responseType=code";
            Console.WriteLine("Opening link " + url);
            Process.Start(new ProcessStartInfo() { FileName = url, UseShellExecute = true });
            Console.Write("Enter authorization code:");
            string AuthCode = Console.ReadLine();
            if (string.IsNullOrEmpty(AuthCode))
            {
                Console.WriteLine("Invalid Auth Code");
                Console.ReadKey();
                return;
            }
            var auth = Auth.GetAuth(fortnitePCGameClient, AuthCode);

            Console.WriteLine("Welcome " + auth.displayName);
            Console.WriteLine("Account ID " + auth.account_id);
            Console.WriteLine("Your access token is: " + auth.access_token);
            Console.WriteLine("Getting QueryProfile");
            File.WriteAllText("auth.json", auth.ToString());
            QueryProfile.Modal.QueryProfileRoot q = QueryProfile.Get(auth, QueryProfile.Profile.athena);


            var Account = q.profileChanges[0].profile;
            List<CosmeticsDB.Datum> ownedCosmetics = new();
            foreach (var item in Account.items)
            {
                string itemName = item.Value.templateId;
                if (itemName.StartsWith("AthenaCharacter") || itemName.StartsWith("AthenaBackpack") || itemName.StartsWith("AthenaDance") || itemName.StartsWith("AthenaPickaxe") || itemName.StartsWith("AthenaGlider"))
                {
                    string id = itemName.Split(":")[1];
                    var cosmetic = cosmetics.data.FirstOrDefault(x => x.id.ToLower() == id);
                    if (cosmetic.introduction == null)
                    {
                        cosmetic.introduction = new CosmeticsDB.Introduction();
                        cosmetic.introduction.text = "Unknown";
                        cosmetic.introduction.backendValue = 0;
                        cosmetic.introduction.season = "1";
                        cosmetic.introduction.chapter = "1";

                    }
                    ownedCosmetics.Add(cosmetic);

                }
            }

            string html = File.ReadAllText("account.html");
            html = html.Replace("{ skins }", GetCards(ownedCosmetics, "outfit"));
            html = html.Replace("{ gliders }", GetCards(ownedCosmetics, "glider"));
            html = html.Replace("{ backblings }", GetCards(ownedCosmetics, "backpack"));
            html = html.Replace("{ pickaxes }", GetCards(ownedCosmetics, "pickaxe"));
            html = html.Replace("{ emotes }", GetCards(ownedCosmetics, "emote"));
            List<QueryProfile.Modal.PastSeason> pastSeasons = Account.stats.attributes.past_seasons;
            int PastWinCount = 0;
            foreach (var season in pastSeasons)
            {
                PastWinCount += season.numWins;
            }

            QueryProfile.Modal.PastSeason currentSeason = new();
            currentSeason.seasonNumber = Account.stats.attributes.season_num;
            currentSeason.numWins = Account.stats.attributes.lifetime_wins - PastWinCount;
            currentSeason.seasonLevel = Account.stats.attributes.level;
            currentSeason.bookLevel = Account.stats.attributes.book_level;
            currentSeason.purchasedVIP = false;


            pastSeasons.Add(currentSeason);
            QueryProfile.Modal.QueryProfileRoot common = QueryProfile.Get(auth, QueryProfile.Profile.common_core);
            html = html.Replace("{ LifeTimeWins }", Account.stats.attributes.lifetime_wins.ToString());
            string VBucks = common.profileChanges[0].profile.items.First(x => x.Value.templateId == "Currency:MtxGiveaway").Value.quantity.ToString();
            string GiftsSent = common.profileChanges[0].profile.stats.attributes.gift_history.num_sent.ToString();
            string GiftsReceived = common.profileChanges[0].profile.stats.attributes.gift_history.num_received.ToString();
            html = html.Replace("{ VBucks }", VBucks);
            html = html.Replace("{ GiftsSent }", GiftsSent);
            html = html.Replace("{ GiftsReceived }", GiftsReceived);
            html = html.Replace("{ stats }", GetStats(pastSeasons));
            html = html.Replace("{ CREATION_TIMESTAMP }", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            html = html.Replace("{DisplayName}", auth.displayName);
            html = html.Replace("{EpicID}", auth.account_id);
            File.WriteAllText($"{auth.account_id}.html", html);

        }

        static string GetCards(IEnumerable<CosmeticsDB.Datum> data, string cosmeticType)
        {
            string cards = "";

            foreach (var item in data.Where(x => x.type.value == cosmeticType).OrderBy(x => x.introduction.backendValue))
            {
                cards += $"skins.push(\"{item.name}|{item.id}|Chapter {item.introduction.chapter}, Season {item.introduction.season}\");";
            }
            return cards;
        }

        static string GetStats(IEnumerable<QueryProfile.Modal.PastSeason> pastSeasons)
        {
            string stats = "";
            foreach (var season in pastSeasons)
            {
                stats += $"stats.push(new Season({season.seasonNumber}, {season.numWins}, {season.seasonLevel}, {season.bookLevel}, {season.purchasedVIP.ToString().ToLower()}));";
            }
            return stats;
        }
    }
}