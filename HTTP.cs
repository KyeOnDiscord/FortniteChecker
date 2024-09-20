namespace FortniteChecker;
public static class HttpClientSingleton
{
    private static readonly HttpClient _httpClient;

    static HttpClientSingleton()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        // Set any other default settings here
    }

    public static HttpClient Instance => _httpClient;
}
