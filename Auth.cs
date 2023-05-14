using System.Text.Json;
namespace FortniteChecker
{
    internal static class Auth
    {
        internal static Modal.AuthRoot GetAuth(AuthClient authClient, string AuthCode)
        {
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authClient.Authorization);
                http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", AuthCode),
                });
                var response = http.PostAsync("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token", content).GetAwaiter().GetResult();
                string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonSerializer.Deserialize<Modal.AuthRoot>(resp);
            }
        }

        internal class Modal
        {
            public class AuthRoot
            {
                public string access_token { get; set; }
                public int expires_in { get; set; }
                public DateTime expires_at { get; set; }
                public string token_type { get; set; }
                public string refresh_token { get; set; }
                public int refresh_expires { get; set; }
                public DateTime refresh_expires_at { get; set; }
                public string account_id { get; set; }
                public string client_id { get; set; }
                public bool internal_client { get; set; }
                public string client_service { get; set; }
                public List<object> scope { get; set; }
                public string displayName { get; set; }
                public string app { get; set; }
                public string in_app_id { get; set; }
                public string device_id { get; set; }
                public string product_id { get; set; }
                public string application_id { get; set; }
            }
        }
    }
}
