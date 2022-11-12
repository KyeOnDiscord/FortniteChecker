using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FortniteChecker
{
    internal static class QueryProfile
    {
        public static JObject Get(JObject auth)
        {
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + auth["access_token"].ToString());
                http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = http.PostAsync("https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/game/v2/profile/" + auth["account_id"] + "/client/QueryProfile?profileId=athena", content).GetAwaiter().GetResult();
                return JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }
    }
}
