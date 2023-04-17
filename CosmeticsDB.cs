namespace FortniteChecker;

internal sealed class CosmeticsDB
{

    public sealed class CosmeticsDBRoot
    {
        public int status { get; set; }
        public Datum[] data { get; set; }
    }


    public sealed class Datum
    {
        public string id { get; set; }
        public Rarity rarity { get; set; }
        public string name { get; set; }
        public Type type { get; set; }
        public string description { get; set; }
        public Series series { get; set; }
        public Set set { get; set; }
        public Introduction introduction { get; set; }
        public Images images { get; set; }
        public Variant[] variants { get; set; }
        public string[] searchTags { get; set; }
        public string[] gameplayTags { get; set; }
        public string[] metaTags { get; set; }
        public string showcaseVideo { get; set; }
        public string dynamicPakId { get; set; }
        public string itemPreviewHeroPath { get; set; }
        public string displayAssetPath { get; set; }
        public string definitionPath { get; set; }
        public string path { get; set; }
        public DateTime added { get; set; }
        public DateTime[] shopHistory { get; set; }
        public string unlockRequirements { get; set; }
        public string exclusiveDescription { get; set; }
        public string customExclusiveCallout { get; set; }
        public string[] builtInEmoteIds { get; set; }

        public bool favourite { get; set; } //self defined
        public List<OwnedVariant> OwnedVariant { get; set; } = new();// self defined
    }

    public sealed class Type
    {
        public string value { get; set; }
        public string displayValue { get; set; }
        public string backendValue { get; set; }
    }

    public sealed class Rarity
    {

        public string value { get; set; }
        public string displayValue { get; set; }
        public string backendValue { get; set; }
        public int backendIntValue { get; set; }//self defined
    }

    public sealed class Series
    {
        public string value { get; set; }
        public string image { get; set; }
        public string[] colors { get; set; }
        public string backendValue { get; set; }
    }

    public sealed class Set
    {
        public string value { get; set; }
        public string text { get; set; }
        public string backendValue { get; set; }
    }

    public sealed class Introduction
    {
        public string chapter { get; set; }
        public string season { get; set; }
        public string text { get; set; }
        public int backendValue { get; set; }

    }

    public sealed class Images
    {
        public string smallIcon { get; set; }
        public string icon { get; set; }
        public string featured { get; set; }
        public Other other { get; set; }
    }

    public sealed class Other
    {
        public string background { get; set; }
        public string coverart { get; set; }
        public string decal { get; set; }
    }

    public sealed class Variant
    {
        public string channel { get; set; }
        public string type { get; set; }
        public Option[] options { get; set; }
    }

    public sealed class Option
    {
        public string tag { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string unlockRequirements { get; set; }
    }
}



internal sealed class Banners
{
    public class Root
    {
        public int status { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string devName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public object category { get; set; }
        public bool fullUsageRights { get; set; }
        public Images images { get; set; }
    }

    public class Images
    {
        public string smallIcon { get; set; }
        public string icon { get; set; }
    }

}
