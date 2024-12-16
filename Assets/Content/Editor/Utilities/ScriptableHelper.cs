using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptableHelper
{
    const string TextDataPath = "Assets/Content/Data/DBDData.tsv";

    const string DataPath = "Assets/Content/Data";
    const string TexturePath = "Assets/Content/Textures";
    const string AddonsPath = "Assets/Content/Data/Addons";
    const string PerksPath = "Assets/Content/Data/Perks";
    const string CharPath = "Assets/Content/Data/Characters";


    [MenuItem("ScriptableHelper/Test")]
    public static void Test()
    {
        Debug.Log(Application.dataPath);
    }

    [MenuItem("ScriptableHelper/Read DbdData File")]
    public static void ReadFile()
    {
        if (!AssetDatabase.IsValidFolder(AddonsPath))
        {
            AssetDatabase.CreateFolder(DataPath, "Addons");
        }

        if (!AssetDatabase.IsValidFolder(PerksPath))
        {
            AssetDatabase.CreateFolder(DataPath, "Perks");
        }
        
        if (!AssetDatabase.IsValidFolder(CharPath))
        {
            AssetDatabase.CreateFolder(DataPath, "Characters");
        }

        StreamReader reader = new StreamReader(TextDataPath);

        string rawFile = reader.ReadToEnd();

        reader.Close();

        string[] splittedLines = rawFile.Split("\n");

        List<Characters> killers = new List<Characters>();
        List<Characters> survivors = new List<Characters>();

        List<Addons> killersAddons = new List<Addons>();

        List<Perks> killersPerks = new List<Perks>();
        List<Perks> survivorPerks = new List<Perks>();

        for (int i = 0; i < splittedLines.Length; i++)
        {
            bool isAddon = true;

            //if (i > 23) break;

            if (splittedLines[i][1] == '#' || splittedLines[i][0] == '§') continue;

            string[] tabSplit = splittedLines[i].Split("\t");

            if (tabSplit.Length != 6) continue;

            string id = tabSplit[1];
            string[] idSplit = id.Split("_");

            string iconPath = TexturePath;
            string assetPath = DataPath;

            string iconCharaPath = TexturePath;
            string assetCharaPath = CharPath;

            string charName = GetCharacterName(idSplit[1]);

            if (charName == "") continue;

            bool hasCharacter = false;

            switch (idSplit[0])
            {
                case "KillerAddon":

                    if (!AssetDatabase.IsValidFolder(AddonsPath + "/Killers"))
                    {
                        AssetDatabase.CreateFolder(AddonsPath, "Killers");
                    }

                    if (!AssetDatabase.IsValidFolder(AddonsPath + "/Killers/" + charName))
                    {
                        AssetDatabase.CreateFolder(AddonsPath + "/Killers", charName);
                    }

                    iconPath += "/Addons/Killers/" + charName + "/";
                    iconPath += "IconAddon_" + FormatName(tabSplit[4]) + ".png";

                    assetPath += "/Addons/Killers/" + charName + "/";
                    assetPath += id;
                    assetPath += ".asset";

                    break;

                case "ItemAddon":

                    if (!AssetDatabase.IsValidFolder(AddonsPath + "/Survivors"))
                    {
                        AssetDatabase.CreateFolder(AddonsPath, "Survivors");
                    }

                    if (!AssetDatabase.IsValidFolder(AddonsPath + "/Survivors/" + idSplit[1]))
                    {
                        AssetDatabase.CreateFolder(AddonsPath + "/Survivors", idSplit[1]);
                    }

                    //iconPath = GetItemAddonPath(idSplit, true);

                    iconPath += "/Addons/Survivors/" + idSplit[1] + "/";
                    iconPath += "IconAddon_" + FormatName(tabSplit[4]) + ".png";


                    //assetPath = GetItemAddonPath(idSplit, false);
                    assetPath += "/Addons/Survivors/" + idSplit[1] + "/";
                    assetPath += id;
                    assetPath += ".asset";

                    break;

                case "KillerPerk":

                    isAddon = false;

                    if (!AssetDatabase.IsValidFolder(PerksPath + "/Killers"))
                    {
                        AssetDatabase.CreateFolder(PerksPath, "Killers");
                    }

                    if (!AssetDatabase.IsValidFolder(PerksPath + "/Killers/" + charName))
                    {
                        AssetDatabase.CreateFolder(PerksPath + "/Killers", charName);
                    }

                    if (!AssetDatabase.IsValidFolder(CharPath + "/Killers"))
                    {
                        AssetDatabase.CreateFolder(CharPath, "Killers");
                    }

                    iconPath += "/Perks/Killers/" + charName + "/";
                    iconPath += "Iconperks_" + FormatName(tabSplit[4]) + ".png";

                    assetPath += "/Perks/Killers/" + charName + "/";
                    assetPath += id;
                    assetPath += ".asset";


                    assetCharaPath += "/Killers/" + charName + ".asset";

                    iconCharaPath += "/Characters/Killers/" + charName + "_Portrait.png";

                    hasCharacter = true;

                    break;

                case "SurvivorPerk":

                    isAddon = false;

                    if (!AssetDatabase.IsValidFolder(PerksPath + "/Survivors"))
                    {
                        AssetDatabase.CreateFolder(PerksPath, "Survivors");
                    }

                    if (!AssetDatabase.IsValidFolder(PerksPath + "/Survivors/" + charName))
                    {
                        AssetDatabase.CreateFolder(PerksPath + "/Survivors", charName);
                    }

                    if (!AssetDatabase.IsValidFolder(CharPath + "/Survivors"))
                    {
                        AssetDatabase.CreateFolder(CharPath, "Survivors");
                    }
                    

                    iconPath += "/Perks/Survivors/" + charName + "/";
                    iconPath += "Iconperks_" + FormatName(tabSplit[4]) + ".png";

                    assetPath += "/Perks/Survivors/" + charName + "/";
                    assetPath += id;
                    assetPath += ".asset";

                    assetCharaPath += "/Survivors/" + charName + ".asset";

                    iconCharaPath += "/Characters/Survivors/" + charName + "_Portrait.png";

                    hasCharacter = true;

                    break;

                default:
                    Debug.LogError("Category " + idSplit[0] + " not recognise on line : " + i.ToString());
                    continue;
            }

            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D));

            if (tex)
            {
                if (isAddon)
                {
                    Addons addon = ScriptableObject.CreateInstance<Addons>();

                    if (File.Exists(Application.dataPath.Replace("Assets", "") + assetPath))
                    {
                        if (tex != null)
                        {
                            addon = (Addons)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Addons));

                            addon = SetupAddon(addon, tex, GetRarityType(tabSplit[3]), tabSplit[5], tabSplit[4]);
                        }
                    }
                    else
                    {
                        if (tex)
                        {
                            addon = SetupAddon(addon, tex, GetRarityType(tabSplit[3]), tabSplit[5], tabSplit[4]);
                        }

                        AssetDatabase.CreateAsset(addon, assetPath);

                    }

                    EditorUtility.SetDirty(addon);


                    if(idSplit[0][0] == 'K')
                    {
                        killersAddons.Add(addon);
                    }
                }
                else
                {
                    Perks perk = ScriptableObject.CreateInstance<Perks>();

                    if (File.Exists(Application.dataPath.Replace("Assets", "") + assetPath))
                    {
                        if (tex != null)
                        {
                            perk = (Perks)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Perks));

                            perk = SetupPerk(perk, tex, tabSplit[5], tabSplit[4]);
                        }
                    }
                    else
                    {
                        if (tex)
                        {
                            perk = SetupPerk(perk, tex, tabSplit[5], tabSplit[4]);
                        }

                        AssetDatabase.CreateAsset(perk, assetPath);

                    }

                    EditorUtility.SetDirty(perk);

                    if (idSplit[0][0] == 'K')
                    {
                        killersPerks.Add(perk);
                    }
                    else
                    {
                        survivorPerks.Add(perk);
                    }
                }
            }
            else
            {
                Debug.LogError("Can't find : " + iconPath);
            }

            if (charName != "Common")
            {
                Texture2D texChara = (Texture2D)AssetDatabase.LoadAssetAtPath(iconCharaPath, typeof(Texture2D));

                if (texChara)
                {
                    Characters character = ScriptableObject.CreateInstance<Characters>();

                    if (File.Exists(Application.dataPath.Replace("Assets", "") + assetCharaPath))
                    {
                        if (texChara != null)
                        {
                            character = (Characters)AssetDatabase.LoadAssetAtPath(assetCharaPath, typeof(Characters));

                            SetupCharacter(character, charName, texChara);
                        }
                    }
                    else
                    {
                        if (texChara != null)
                        {
                            SetupCharacter(character, charName, texChara);

                            AssetDatabase.CreateAsset(character, assetCharaPath);
                        }
                    }

                    if (charName[0] == 'K')
                    {
                        killers.Add(character);
                    }
                    else
                    {
                        survivors.Add(character);
                    }

                    EditorUtility.SetDirty(character);
                }
                else
                {
                    if(hasCharacter) Debug.LogError("Can't find : " + iconCharaPath);
                }
            }
        }

        for (int i = 0; i < killers.Count; i++)
        {
            killers[i].KillerAddons = new List<Addons>();
            killers[i].UniquePerks = new List<Perks>();

            for (int y = 0; y < killersAddons.Count; y++)
            {
                if(killersAddons[y].name.Split('_')[1] ==  killers[i].name.Replace("_", ""))
                {
                    killers[i].KillerAddons.Add(killersAddons[y]);
                }
            }

            for (int y = 0; y < killersPerks.Count; y++)
            {
                if (killersPerks[y].name.Split('_')[1] == killers[i].name.Replace("_", ""))
                {
                    killers[i].UniquePerks.Add(killersPerks[y]);
                }
            }

            EditorUtility.SetDirty(killers[i]);
        }

        for (int i = 0; i < survivors.Count; i++)
        {
            survivors[i].UniquePerks = new List<Perks>();

            for (int y = 0; y < survivorPerks.Count; y++)
            {
                if (survivorPerks[y].name.Split('_')[1] == survivors[i].name.Replace("_", ""))
                {
                    survivors[i].UniquePerks.Add(survivorPerks[y]);
                }
            }

            EditorUtility.SetDirty(survivors[i]);
        }

        AssetDatabase.SaveAssets();
    }

    static string GetItemAddonPath(string[] idsplit, bool isIcon)
    {
        string iconPath = isIcon ? TexturePath : DataPath;

        if (!AssetDatabase.IsValidFolder(AddonsPath + "/Survivors/" + idsplit[1]))
        {
            AssetDatabase.CreateFolder(AddonsPath + "/Survivors", idsplit[1]);
        }

        iconPath += "/Addons/Survivors/" + idsplit[1] + "/";
        if (isIcon) 
        {
            iconPath += "Iconaddon_" + idsplit[3] + ".png";
        }
        else
        {
            string name = "";

            for (int i = 0; i < idsplit.Length; i++)
            {
                name += idsplit[i] + "_";
            }
            iconPath += name.Remove(name.Length - 1) + ".asset";
        }

        return iconPath;
    }

    static string GetCharacterName(string idSplit)
    {
        if(idSplit == "Common")
        {
            return idSplit;
        }
        else
        {
            string kId = idSplit.Substring(0, 3);
            string kName = idSplit.Substring(3);

            return kId + "_" + kName;
        }
    }

    static List<char> specialCharacters = new List<char>()
    {
        ' ',
        '"',
        '\'',
        '-',
        '.',
        ',',
        '%',
        '(',
        ')',
        '&',
        ':',
        '!',
        '?'
    };

    static char CheckAccents(char c)
    {
        switch (c)
        {
            case 'é':
                return 'e';

            case 'è':
                return 'e';

            case 'à':
                return 'a';

            case 'â':
                return 'a';
                
            default:
                return c;
        }
    }

    static string FormatName(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        //newText.Append(text[0]);
        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];

            if (specialCharacters.Contains(character)) continue;

            character = CheckAccents(character);

            if (char.IsUpper(character))
            {
                newText.Append(char.ToLower(character));
            }
            else
            {
                newText.Append(character);
            }
        }
        return newText.ToString();
    }

    static Sprite GetSpriteFromTex(Texture2D tex)
    {
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
        if (data != null)
        {
            foreach (Object obj in data)
            {
                if (obj.GetType() == typeof(Sprite))
                {
                    return obj as Sprite;
                }
            }
        }

        return null;
    }

    static Rarity.RarityTypes GetRarityType(string idSplitted)
    {
        Rarity.RarityTypes rarity = Rarity.RarityTypes.Common;

        switch (idSplitted)
        {
            default:
                break;

            case "Common":
                rarity = Rarity.RarityTypes.Common;
                break;

            case "Uncommon":
                rarity = Rarity.RarityTypes.Uncommon;
                break;

            case "Rare":
                rarity = Rarity.RarityTypes.Rare;
                break;

            case "Very Rare":
                rarity = Rarity.RarityTypes.VeryRare;
                break;

            case "Ultra Rare":
                rarity = Rarity.RarityTypes.UltraRare;
                break;
        }

        return rarity;
    }

    static string GetDescription(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        StringBuilder newText = new StringBuilder(text.Length * 2);

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '$')
            {
                if(i + 1 < text.Length)
                {
                    switch (text[i + 1])
                    {
                        default:
                            break;

                        case 'n':
                            newText.Append("\n");
                            break;

                        case 't':
                            newText.Append("\t");
                            break;
                    }
                    i++;
                    continue;
                }
            }
            else
            {
                newText.Append(text[i]);
            }
        }
        return newText.ToString();
    }

    static Addons SetupAddon(Addons addon, Texture2D tex, Rarity.RarityTypes rarity, string description, string name)
    {
        addon.Icon = GetSpriteFromTex(tex);

        addon.Name = name;

        addon.RarityType = rarity;

        addon.Descriton = GetDescription(description);

        return addon;
    }

    static Perks SetupPerk(Perks perk, Texture2D tex, string description, string name)
    {
        perk.Icon = GetSpriteFromTex(tex);

        perk.Name = name;

        perk.Descriton = GetDescription(description);

        return perk;
    }

    static Characters SetupCharacter(Characters chara, string name, Texture2D tex)
    {
        chara.SetName(name);

        if (name[0] == 'K')
        {
            chara.Type = Characters.CharacterType.Killers;
        }
        else
        {
            chara.Type = Characters.CharacterType.Survivors;
        }

        chara.Icon = GetSpriteFromTex(tex);

        return chara;
    }
}
