using System.Text;
using System.Text.Json;

namespace FortniteChecker
{
    internal static class QueryProfile
    {

        internal static Modal.QueryProfileRoot Get(Auth.Modal.AuthRoot auth)
        {
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {auth.access_token}");
                http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = http.PostAsync($"https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/game/v2/profile/{auth.account_id}/client/QueryProfile?profileId=athena", content).GetAwaiter().GetResult();
                string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonSerializer.Deserialize(resp, SourceGenerationContext.Default.QueryProfileRoot);
            }
        }

        internal sealed class Modal
        {
            internal class QueryProfileRoot
            {
                public int profileRevision { get; set; }
                public string profileId { get; set; }
                public int profileChangesBaseRevision { get; set; }
                public List<ProfileChange> profileChanges { get; set; }
                public int profileCommandRevision { get; set; }
                public DateTime serverTime { get; set; }
                public int responseVersion { get; set; }
            }

            internal class ProfileChange
            {
                public string changeType { get; set; }
                public Profile profile { get; set; }
            }

            internal class Profile
            {
                public string _id { get; set; }
                public DateTime created { get; set; }
                public DateTime updated { get; set; }
                public int rvn { get; set; }
                public int wipeNumber { get; set; }
                public string accountId { get; set; }
                public string profileId { get; set; }
                public string version { get; set; }
                public Dictionary<string, Item> items { get; set; }
                public Stats stats { get; set; }
                public int commandRevision { get; set; }
            }

            internal class Stats
            {
                public Attributes attributes { get; set; }
            }
            internal class Item
            {
                public string templateId { get; set; }
                public Attributes attributes { get; set; }
                public int quantity { get; set; }
            }

            internal class Attributes
            {


            }
        }
    }
}
