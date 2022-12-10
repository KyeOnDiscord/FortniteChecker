using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;

namespace FortniteChecker;

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
    static void Main()
    {
        HttpClient httpClient = new HttpClient();

        //Create fortnitePCGameClient
        var fortnitePCGameClient = new AuthClient { ClientID = "ec684b8c687f479fadea3cb2ad83f5c6", Secret = "e1f31c211f28413186262d37a13fc84d" };

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

        if (auth.access_token == null)
        {
            Console.WriteLine("Invalid Access Token");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Welcome " + auth.displayName);
#if DEBUG
        Console.WriteLine("Account ID " + auth.account_id);
        Console.WriteLine("Your access token is: " + auth.access_token);
        Console.WriteLine("Getting QueryProfile");
        // File.WriteAllText("auth.json", auth.ToString());
#endif
        QueryProfile.Modal.QueryProfileRoot q = QueryProfile.Get(auth, QueryProfile.Profile.athena);


        var Account = q.profileChanges[0].profile;
        Console.WriteLine("Fetching all Fortnite cosmetics");
        string AllCosmetics = httpClient.GetStringAsync("https://fortnite-api.com/v2/cosmetics/br/").GetAwaiter().GetResult();
        CosmeticsDB.CosmeticsDBRoot cosmetics = JsonSerializer.Deserialize(AllCosmetics, SourceGenerationContext.Default.CosmeticsDBRoot);
        Console.WriteLine("Fetched all cosmetics, count: " + cosmetics.data.Length);

        List<CosmeticsDB.Datum> ownedCosmetics = new();
        string[] CosmeticItemsToSearch = { "AthenaCharacter", "AthenaBackpack", "AthenaDance", "AthenaPickaxe", "AthenaGlider", "AthenaItemWrap", "AthenaLoadingScreen", "AthenaMusicPack", "AthenaSkyDiveContrail", "AthenaSpray" };
        foreach (var item in Account.items)
        {
            string itemName = item.Value.templateId;

            string itemType = itemName.Split(':')[0];
            string itemValue = itemName.Split(':')[1];
            if (CosmeticItemsToSearch.Contains(itemType))
            {
                var cosmetic = cosmetics.data.FirstOrDefault(x => x.id.ToLower() == itemValue);
                if (cosmetic == null)
                    continue;

                if (cosmetic.introduction == null)
                {
                    cosmetic.introduction = new CosmeticsDB.Introduction();
                    cosmetic.introduction.text = "Unknown";
                    cosmetic.introduction.backendValue = 0;
                    cosmetic.introduction.season = "1";
                    cosmetic.introduction.chapter = "1";
                }
                cosmetic.rarity.backendIntValue = Rarity.RarityToInt(cosmetic.rarity.value);
                ownedCosmetics.Add(cosmetic);

                if (item.Value.attributes.variants != null)
                {
                    foreach (var variant in item.Value.attributes.variants)
                    {
                        foreach (string stage in variant.owned)
                        {
                            string StageName = stage;
                            if (stage.Contains("."))
                                StageName = stage.Split('.')[1]; //For something like JerseyColor.011 to 011

                            if (StageName.Length == 32 || StageName == "RichColor") //If the stage name is something like RichColor.3DBD24153DC7D31D000000003F800000 which fortnite-api doesn't support
                                continue;

                            cosmetic.OwnedVariant.Add(new OwnedVariant(StageName, variant.channel));
                        }
                    }
                }
                if (item.Value.attributes.favorite)
                {
                    cosmetic.favourite = true;
                }
            }
        }
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

        int VBucks = GetVbucks(common);
        string GiftsSent = common.profileChanges[0].profile.stats.attributes.gift_history.num_sent.ToString();
        string GiftsReceived = common.profileChanges[0].profile.stats.attributes.gift_history.num_received.ToString();

        string EpicID = GetHashString(auth.account_id).Substring(0, 12);

        string lifetimewins = Account.stats.attributes.lifetime_wins.ToString();
        string timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        string data = JsonBuilder.CreateJsonFile(ownedCosmetics, pastSeasons, VBucks.ToString(), GiftsSent, GiftsReceived, timestamp, auth.displayName, lifetimewins, EpicID);

        HttpContent content = new StringContent(data, Encoding.UTF8, "text/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
#if DEBUG

        HttpResponseMessage resp = httpClient.PostAsync("http://localhost:3000/file", content).GetAwaiter().GetResult();
        string messageID = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        string CheckerLink = $"http://localhost:3000/?file={messageID}";
        Process.Start(new ProcessStartInfo() { FileName = CheckerLink, UseShellExecute = true });
#endif

#if RELEASE
        HttpResponseMessage resp = httpClient.PostAsync("https://checker.proswapper.xyz/file", content).GetAwaiter().GetResult();
        string messageID = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        string CheckerLink = $"https://checker.proswapper.xyz/?file={messageID}";
        CheckerLink = "https://link-to.net/86737/" + new Random().Next(0, 1000).ToString() + "/dynamic/?r=" + Base64Encode(CheckerLink);
        Process.Start(new ProcessStartInfo() { FileName = CheckerLink, UseShellExecute = true });
#endif



    }

    private static int GetVbucks(QueryProfile.Modal.QueryProfileRoot common)
    {
        int VBucks = 0;
        foreach (var item in common.profileChanges[0].profile.items)
        {
            if (item.Value.templateId.StartsWith("Currency:Mtx"))
            {
                if (item.Value != null && item.Value.quantity != null)
                {
                    VBucks += (int)item.Value.quantity;
                }
            }
        }
        return VBucks;
    }

    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}