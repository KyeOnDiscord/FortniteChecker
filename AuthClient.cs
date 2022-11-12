using System.Text;

namespace FortniteChecker
{
    internal class AuthClient
    {
        public AuthClient(string ClientID, string Secret)
        {
            this.ClientID = ClientID;
            this.Secret = Secret;
        }
        public string ClientID { get; }
        public string Secret { get; }
        public string Authorization => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientID}:{Secret}"))}";
    }
}
