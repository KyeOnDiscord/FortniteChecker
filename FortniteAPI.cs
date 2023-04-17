using MessagePack;
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
            byte[] buffer = resp.Content.ReadAsByteArrayAsync().Result;
            Console.WriteLine("Decompressing cosmetics, " + buffer.LongLength.ToSize(MyExtension.SizeUnits.MB) + "MB");
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithSecurity(MessagePackSecurity.UntrustedData);
            string json = MessagePackSerializer.ConvertToJson(buffer, lz4Options);
            var cosmetics = JsonSerializer.Deserialize<CosmeticsDB.CosmeticsDBRoot>(json);
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
            byte[] buffer = resp.Content.ReadAsByteArrayAsync().Result;
            Console.WriteLine("Decompressing banners, " + buffer.LongLength.ToSize(MyExtension.SizeUnits.MB) + "MB");
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithSecurity(MessagePackSecurity.UntrustedData);
            string json = MessagePackSerializer.ConvertToJson(buffer, lz4Options);
            var cosmetics = JsonSerializer.Deserialize<Banners.Root>(json);
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