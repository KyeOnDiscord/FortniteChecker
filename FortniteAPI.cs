using System.Text.Json;
namespace FortniteChecker;

internal static class FortniteAPI
{

    internal static T DownloadData<T>(string URL)
    {
        HttpResponseMessage resp = HttpClientSingleton.Instance.GetAsync(URL).GetAwaiter().GetResult();
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

    internal static CosmeticsDB.CosmeticsDBRoot GetAllCosmetics()
    {
        Console.WriteLine("Fetching all Fortnite cosmetics");
        HttpResponseMessage resp = HttpClientSingleton.Instance.GetAsync(FortniteAPI_Endpoints.Cosmetics).GetAwaiter().GetResult();
        if (resp.IsSuccessStatusCode)
        {
            string buffer = resp.Content.ReadAsStringAsync().Result;
            CosmeticsDB.CosmeticsDBRoot? cosmetics = JsonSerializer.Deserialize<CosmeticsDB.CosmeticsDBRoot?>(buffer);
            if (cosmetics != null)
            {
                Console.WriteLine("Fetched all cosmetics, count: " + cosmetics.data.Length);
                return cosmetics;
            }
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

    internal static Banners.Root GetBanners()
    {
        Console.WriteLine("Fetching all banners");
        HttpResponseMessage resp = HttpClientSingleton.Instance.GetAsync(FortniteAPI_Endpoints.Banners).GetAwaiter().GetResult();
        if (resp.IsSuccessStatusCode)
        {
            string buffer = resp.Content.ReadAsStringAsync().Result;
            Banners.Root? cosmetics = JsonSerializer.Deserialize<Banners.Root?>(buffer);
            if (cosmetics != null)
            {
                Console.WriteLine("Fetched all banners, count: " + cosmetics.data.Length);
                return cosmetics;
            }
            else
                throw new Exception("Banners were null");
        }
        else
        {
            string error = $"Failed to get banners from FortniteAPI, status code: {resp.StatusCode}";
            Console.WriteLine(error);
            throw new Exception(error);
        }
    }
}