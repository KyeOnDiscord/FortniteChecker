namespace FortniteChecker;

internal static class FortniteAPI_Endpoints
{
    private const string BASE_URL = "https://fortnite-api.com";
    public static readonly string Cosmetics = BASE_URL + "/v2/cosmetics/br?responseOptions=ignore_null";
    public static readonly string Banners = BASE_URL + "/v1/banners?responseOptions=ignore_null";
}
