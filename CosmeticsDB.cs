using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace FortniteChecker
{
    internal class CosmeticsDB
    {

        public class Rootobject
        {
            public int status { get; set; }
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public JToken jToken { get; set; }//The account's skin itself, can hold information like variants
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public Type type { get; set; }
            public Rarity rarity { get; set; }
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
        }

        public class Type
        {
            public string value { get; set; }
            public string displayValue { get; set; }
            public string backendValue { get; set; }
        }

        public class Rarity
        {
            public string value { get; set; }
            public string displayValue { get; set; }
            public string backendValue { get; set; }
        }

        public class Series
        {
            public string value { get; set; }
            public string image { get; set; }
            public string[] colors { get; set; }
            public string backendValue { get; set; }
        }

        public class Set
        {
            public string value { get; set; }
            public string text { get; set; }
            public string backendValue { get; set; }
        }

        public class Introduction
        {
            public string chapter { get; set; }
            public string season { get; set; }
            public string text { get; set; }
            public int backendValue { get; set; }

        }

        public class Images
        {
            public string smallIcon { get; set; }
            public string icon { get; set; }
            public string featured { get; set; }
            public Other other { get; set; }
        }

        public class Other
        {
            public string background { get; set; }
            public string coverart { get; set; }
            public string decal { get; set; }
        }

        public class Variant
        {
            public string channel { get; set; }
            public string type { get; set; }
            public Option[] options { get; set; }
        }

        public class Option
        {
            public string tag { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string unlockRequirements { get; set; }
        }
    }
}
