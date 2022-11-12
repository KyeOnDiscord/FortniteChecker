using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FortniteChecker
{
    internal class Program
    {
        //Compile with native with dotnet publish -r win-x64 -c Release
        static unsafe void Main(string[] args)
        {

            //Create fortnitePCGameClient
            var fortnitePCGameClient = new AuthClient("ec684b8c687f479fadea3cb2ad83f5c6", "e1f31c211f28413186262d37a13fc84d");
            CosmeticsDB.Rootobject cosmetics = JsonConvert.DeserializeObject<CosmeticsDB.Rootobject>(File.ReadAllText("br.json"));
            //Open web link
            string url = $"https://www.epicgames.com/id/api/redirect?clientId={fortnitePCGameClient.ClientID}&responseType=code";
            Process.Start(new ProcessStartInfo() { FileName = url, UseShellExecute = true });
            Console.Write("Enter authorization code:");
            string AuthCode = Console.ReadLine();

            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", fortnitePCGameClient.Authorization);
                http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", AuthCode),
                });
                var response = http.PostAsync("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token", content).GetAwaiter().GetResult();
                string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JObject auth = JObject.Parse(resp);
                Console.WriteLine("Welcome " + auth["displayName"]);
                Console.WriteLine("Account ID " + auth["account_id"]);
                Console.WriteLine("Your access token is: " + auth["access_token"]);
                var q = QueryProfile.Get(auth);
                var items = q["profileChanges"][0]["profile"]["items"].ToArray();


                List<CosmeticsDB.Datum> ownedCosmetics = new();
                foreach (JToken arrayitem in items)
                {
                    var item = arrayitem.Children().Children().ToList();
                    string itemName = item[0].First().ToString();

                    if (itemName.StartsWith("AthenaCharacter") || itemName.StartsWith("AthenaBackpack") || itemName.StartsWith("AthenaDance") || itemName.StartsWith("AthenaPickaxe") || itemName.StartsWith("AthenaGlider"))
                    {
                        string cid = itemName.Split(":")[1];
                        var cosmetic = cosmetics.data.FirstOrDefault(x => x.id.ToLower() == cid);
                        if (cosmetic.introduction == null)
                        {
                            cosmetic.introduction = new CosmeticsDB.Introduction();
                            cosmetic.introduction.text = "Unknown";
                            cosmetic.introduction.backendValue = 0;
                            cosmetic.introduction.season = "Unknown";

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
                html = html.Replace("{ CREATION_TIMESTAMP }", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                html = html.Replace("{DisplayName}", auth["displayName"].ToString());
                html = html.Replace("{EpicID}", auth["account_id"].ToString());
                File.WriteAllText($"{auth["account_id"]}.html", html);
            }
        }

        static string GetCards(IEnumerable<CosmeticsDB.Datum> data, string cosmeticType)
        {
            string cards = "";

            foreach (var item in data.Where(x => x.type.value == cosmeticType).OrderBy(x => x.introduction.backendValue))
            {
                cards += $"skins.push(\"{item.name}|{item.images.icon}|Chapter {item.introduction.chapter}, Season {item.introduction.season}\");";
            }
            return cards;
        }
    }
}