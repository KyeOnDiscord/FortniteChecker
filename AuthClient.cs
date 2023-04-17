namespace FortniteChecker;

internal sealed class AuthClient
{
    public string ClientID { get; init; }
    public string Secret { get; init; }
    public string Authorization => $"Basic {Program.Base64Encode($"{ClientID}:{Secret}")}";
}