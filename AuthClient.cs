using System.Text;

namespace FortniteChecker
{
    internal sealed class AuthClient
    {
        public required string ClientID { get; init; }
        public required string Secret { get; init; }
        public string Authorization => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientID}:{Secret}"))}";
    }
}
