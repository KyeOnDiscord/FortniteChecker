using System.Text.Json;
namespace FortniteChecker;

internal static class FortniteAPI
{
    private const string BASE_URL = "https://fortnite-api.com";
    public static readonly string Cosmetics = BASE_URL + "/v2/cosmetics/br";
    public static readonly string Cars = BASE_URL + "/v2/cosmetics/cars";
    public static readonly string Banners = BASE_URL + "/v1/banners";
    internal static async Task<T> DownloadDataAsync<T>(string URL)
    {
        HttpResponseMessage resp = await HttpClientSingleton.Instance.GetAsync(URL);
        if (resp.IsSuccessStatusCode)
        {
            string buffer = resp.Content.ReadAsStringAsync().Result;
            T? cosmetics = JsonSerializer.Deserialize<T?>(buffer);
            if (cosmetics != null)
                return cosmetics;
            else
                throw new Exception("Downloaded Cosmetics was null");
        }
        else
        {
            string error = $"Failed to get cosmetics from FortniteAPI, status code: {resp.StatusCode}";
            Console.WriteLine(error);
            throw new Exception(error);
        }
    }
}