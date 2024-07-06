using System.Text.Json;

namespace FortniteChecker;

internal static class FortniteAPI
{
    internal static CosmeticsDB.CosmeticsDBRoot GetAllCosmetics()
    {
        Console.WriteLine("Fetching all Fortnite cosmetics");

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage resp = httpClient.GetAsync(Endpoints.FortniteAPIURL).GetAwaiter().GetResult();
        if (resp.IsSuccessStatusCode)
        {
            string buffer = resp.Content.ReadAsStringAsync().Result;
            var cosmetics = JsonSerializer.Deserialize<CosmeticsDB.CosmeticsDBRoot>(buffer);
            Console.WriteLine("Fetched all cosmetics, count: " + cosmetics.data.Length);
            return cosmetics;
        }
        else
        {
            Console.WriteLine($"Failed to get cosmetics from FortniteAPI, status code: {resp.StatusCode}");
            return null;
        }
    }

    internal static Banners.Root GetBanners()
    {
        Console.WriteLine("Fetching all banners");

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage resp = httpClient.GetAsync(Endpoints.Banners).GetAwaiter().GetResult();
        if (resp.IsSuccessStatusCode)
        {
            string buffer = resp.Content.ReadAsStringAsync().Result;
            var cosmetics = JsonSerializer.Deserialize<Banners.Root>(buffer);
            Console.WriteLine("Fetched all banners, count: " + cosmetics.data.Length);
            return cosmetics;
        }
        else
        {
            Console.WriteLine($"Failed to get banners from FortniteAPI, status code: {resp.StatusCode}");
            return null;
        }
    }
}