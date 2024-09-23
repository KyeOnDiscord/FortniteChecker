using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
namespace FortniteChecker;

internal class Program
{
    static async Task TypeMessageAsync(string message, int delay)
    {
        for (int i = 0; i < message.Length; i += 3)
        {
            // Get the next three characters (or fewer if at the end)
            string chunk = message.Substring(i, Math.Min(3, message.Length - i));

            Console.Write(chunk);
            await Task.Delay(delay);
        }
    }

    static async Task Main()
    {
        Task<CosmeticsDB.CosmeticsDBRoot> cosmeticsTask = FortniteAPI.DownloadDataAsync<CosmeticsDB.CosmeticsDBRoot>(FortniteAPI.Cosmetics);
        Task<CosmeticsDB.CosmeticsDBRoot> carsTask = FortniteAPI.DownloadDataAsync<CosmeticsDB.CosmeticsDBRoot>(FortniteAPI.Cars);
        Task<Banners.Root> bannersTask = FortniteAPI.DownloadDataAsync<Banners.Root>(FortniteAPI.Banners);



        Console.Title = "Kye's Fortnite Account Checker";
        Console.ForegroundColor = ConsoleColor.Green;
        string asciiArt = @"
 _____          _         _ _          ____ _               _             
|  ___|__  _ __| |_ _ __ (_) |_ ___   / ___| |__   ___  ___| | _____ _ __ 
| |_ / _ \| '__| __| '_ \| | __/ _ \ | |   | '_ \ / _ \/ __| |/ / _ \ '__|
|  _| (_) | |  | |_| | | | | ||  __/ | |___| | | |  __/ (__|   <  __/ |   
|_|  \___/|_|   \__|_| |_|_|\__\___|  \____|_| |_|\___|\___|_|\_\___|_|   
";
        await TypeMessageAsync(asciiArt, 2);
        var cosmetics = cosmeticsTask.Result;
        var banners = bannersTask.Result;
        var cars = carsTask.Result;
        cosmetics.data.AddRange(cars.data);
        Console.WriteLine("\nKye's Fortnite Account Checker (September 2024 Update)\n");
        await Task.WhenAll(cosmeticsTask, bannersTask);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Press any key to open browser and authenticate your Epic Games account ...");
        Console.ReadKey(); // Wait for user input
        Console.ForegroundColor = ConsoleColor.Gray;

        //Open web link
        string url = $"https://www.epicgames.com/id/api/redirect?clientId={AuthClients.fortnitePCGameClient.ClientID}&responseType=code";
        Console.WriteLine("Opening link " + url);
        Process.Start(new ProcessStartInfo() { FileName = url, UseShellExecute = true });
        Console.Write("Enter authorization code:");
        string? AuthCode = Console.ReadLine();
        if (string.IsNullOrEmpty(AuthCode))
        {
            Console.WriteLine("Invalid Auth Code");
            Console.ReadKey();
            return;
        }
        var auth = Auth.GetAuth(AuthClients.fortnitePCGameClient, AuthCode);

        if (auth.access_token == null)
        {
            Console.WriteLine("Invalid Access Token");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Welcome " + auth.displayName);
        Console.WriteLine("Account ID " + auth.account_id);
#if DEBUG

        Console.WriteLine("Your access token is: " + auth.access_token);
        Console.WriteLine("Getting QueryProfile");
        // File.WriteAllText("auth.json", auth.ToString());
#endif
        QueryProfile.Modal.QueryProfileRoot q = QueryProfile.Get(auth, QueryProfile.Profile.athena);

        var Account = q.profileChanges[0].profile;

        List<CosmeticsDB.Datum> ownedCosmetics = new();
        string[] CosmeticItemsToSearch = { "AthenaCharacter", "AthenaBackpack", "AthenaDance", "AthenaPickaxe", "AthenaGlider", "AthenaItemWrap", "AthenaLoadingScreen", "AthenaMusicPack", "AthenaSkyDiveContrail", "VehicleCosmetics_Body" };
        foreach (var item in Account.items)
        {
            string[] itemName = item.Value.templateId.Split(':');

            string itemType = itemName[0];
            string itemValue = itemName[1];
            if (CosmeticItemsToSearch.Contains(itemType))
            {
                var cosmetic = cosmetics.data.FirstOrDefault(x => x.id.ToLower() == itemValue);
                if (cosmetic == null)
                    continue;

                if (cosmetic.introduction == null)
                {
                    cosmetic.introduction = new()
                    {
                        text = "Unknown",
                        backendValue = 0,
                        season = "1",
                        chapter = "1"
                    };
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
        currentSeason.purchasedVIP = Account.stats.attributes.book_purchased;


        pastSeasons.Add(currentSeason);
        QueryProfile.Modal.QueryProfileRoot common = QueryProfile.Get(auth, QueryProfile.Profile.common_core);

        int VBucks = GetVbucks(common);


        var OwnedBanners = GetBanners(common);
        List<Banners.Datum> ActuallyOwnedBanners = new();

        foreach (var item in OwnedBanners)
        {
            var banner = banners.data.FirstOrDefault(x => x.id.ToLower() == item.ToLower());
            if (banner != null)
                ActuallyOwnedBanners.Add(banner);
        }

        string GiftsSent = "0";
        string GiftsReceived = "0";

        if (common.profileChanges[0].profile.stats.attributes.gift_history != null)
        {
            GiftsSent = common.profileChanges[0].profile.stats.attributes.gift_history.num_sent.ToString();
            GiftsReceived = common.profileChanges[0].profile.stats.attributes.gift_history.num_received.ToString();
        }


        string EpicID = GetHashString(auth.account_id).Substring(0, 12);

        string lifetimewins = Account.stats.attributes.lifetime_wins.ToString();
        string timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var data = JsonBuilder.CreateJsonFile(ownedCosmetics, pastSeasons, ActuallyOwnedBanners, VBucks.ToString(), GiftsSent, GiftsReceived, timestamp, auth.displayName, lifetimewins, EpicID);

        try
        {


#if DEBUG
            string web_url = "http://localhost:3000";
#elif RELEASE
            string web_url = "https://checker.proswapper.xyz";
#endif

            //Send the skin data to the database
            HttpResponseMessage resp = HttpClientSingleton.Instance.PostAsJsonAsync(web_url + "/submit", data).GetAwaiter().GetResult();
            if (resp.StatusCode == HttpStatusCode.OK)
            {
#if DEBUG
                string CheckerLink = $"{web_url}/?bstid={EpicID}";
#elif RELEASE
                string CheckerLink = $"https://bstlar.com/1c/fnchecker?bstid={EpicID}";
#endif

                Console.WriteLine("Opening " + CheckerLink);
                Process.Start(new ProcessStartInfo() { FileName = CheckerLink, UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.ReadKey();
    }

    private static int GetVbucks(QueryProfile.Modal.QueryProfileRoot common)
    {
        int VBucks = 0;
        foreach (var item in common.profileChanges[0].profile.items)
        {
            //MtxComplimentary
            //MtxGiveaway
            //MtxPurchaseBonus
            //MtxPurchased

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

    private static List<string> GetBanners(QueryProfile.Modal.QueryProfileRoot common)
    {
        List<string> Banners = new();
        foreach (var item in common.profileChanges[0].profile.items)
        {

            if (item.Value.templateId.StartsWith("HomebaseBannerIcon:"))
            {
                Banners.Add(item.Value.templateId.Split(':')[1]);
            }
        }
        return Banners;
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

    public static string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
}