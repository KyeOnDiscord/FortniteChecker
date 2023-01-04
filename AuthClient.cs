namespace FortniteChecker;

internal sealed class AuthClient
{
    public required string ClientID { get; init; }
    public required string Secret { get; init; }
    public string Authorization => $"Basic {Program.Base64Encode($"{ClientID}:{Secret}")}";
}