using System;
using StardewModdingAPI;

namespace BirthdayEvent
{
    public class BirthdayLetter : IAssetEditor
    {
        public BirthdayLetter()
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["Birthday"] = "Hello @... ^I wanted to do something special for your birthday, so here it is I figured out how to send a mail :). ^I hope this makes you smile.";
        }
    }
}

