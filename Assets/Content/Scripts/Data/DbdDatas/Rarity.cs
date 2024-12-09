using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Rarity
{
    public enum RarityTypes
    {
        Common,
        Uncommon,
        Rare,
        VeryRare,
        UltraRare
    }

    static public string GetAddonRarity(RarityTypes rarity)
    {
        string outstring = "Assets/Content/Textures/Misc/Addons_BG_";

        switch (rarity)
        {
            case RarityTypes.Common:
                outstring += "Common";
                break;
            case RarityTypes.Uncommon:
                outstring += "Uncommon";
                break;
            case RarityTypes.Rare:
                outstring += "Rare";
                break;
            case RarityTypes.VeryRare:
                outstring += "VeryRare";
                break;
            case RarityTypes.UltraRare:
                outstring += "UltraRare";
                break;
        }

        outstring += ".png";

        return outstring;
    }
}
