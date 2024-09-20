namespace FortniteChecker;

internal class AuthClient
{
    public string ClientID { get; init; }
    public string Secret { get; init; }
    public string Authorization => $"Basic {Program.Base64Encode($"{ClientID}:{Secret}")}";
}

internal static class AuthClients
    {
        public static readonly AuthClient fortnitePCGameClient = new()
        {
            ClientID = "ec684b8c687f479fadea3cb2ad83f5c6",
            Secret = "e1f31c211f28413186262d37a13fc84d"
        };
    }