namespace FortniteChecker
{
    internal class OwnedVariant
    {
        public string Stage { get; init; }
        public string Channel { get; init; }
        public OwnedVariant(string stage, string channel)
        {
            Stage = stage;
            Channel = channel;
        }
    }
}