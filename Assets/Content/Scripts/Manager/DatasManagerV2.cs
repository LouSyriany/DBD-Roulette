using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-20)]
public class DatasManagerV2 : MonoBehaviour
{
    public static DatasManagerV2 Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Instance = this;
        }

        LoadUserData();
    }

    [Serializable]
    public class SavedIntData
    {
        public string Key = "";
        public int Value = -1;
    }

    [Serializable]
    public class SavedStringData
    {
        public string Key = "";
        public string Value = "";
    }

    [Serializable]
    public class SavedBoolData
    {
        public string Key = "";
        public bool Value = true;
    }

    [Serializable]
    public class SavedFloatData
    {
        public string Key = "";
        public float Value = -1f;
    }

    [Serializable]
    public class DataState
    {
        public BaseScriptable Ref;

        public bool State;
    }

    [Serializable]
    public class SettingState
    {
        public SettingsScriptable Ref;

        public bool BoolValue;
        public int IntValue;
        public string StringValue;
        public float FloatValue;

        public void Reset()
        {
            if (Ref != null)
            {
                if (Ref is BoolData)
                {
                    BoolData tmp = Ref as BoolData;
                    BoolValue = tmp.DefaultValue;
                }

                if (Ref is FloatData)
                {
                    FloatData tmp = Ref as FloatData;
                    FloatValue = tmp.DefaultValue;
                }

                if (Ref is IntData)
                {
                    IntData tmp = Ref as IntData;
                    IntValue = tmp.DefaultValue;
                }

                if (Ref is StringData)
                {
                    StringData tmp = Ref as StringData;
                    StringValue = tmp.DefaultValue;
                }
            }
        }
    }

    [Serializable]
    public class Data
    {
        public List<DataState> Survivors        = new List<DataState>();
        public List<DataState> SurvivorPerks    = new List<DataState>();

        public List<DataState> Killers          = new List<DataState>();
        public List<DataState> KillerPerks      = new List<DataState>();
        public List<DataState> KillerAddons     = new List<DataState>();

        public List<DataState> Items            = new List<DataState>();
        public List<DataState> ItemsAddon       = new List<DataState>();

        public void Reset()
        {
            foreach (var item in Survivors)
            {
                item.State = true;
            }
            foreach (var item in SurvivorPerks)
            {
                item.State = true;
            }

            foreach (var item in Killers)
            {
                item.State = true;
            }
            foreach (var item in KillerPerks)
            {
                item.State = true;
            }
            foreach (var item in KillerAddons)
            {
                item.State = true;
            }

            foreach (var item in Items)
            {
                item.State = true;
            }
            foreach (var item in ItemsAddon)
            {
                item.State = true;
            }
        }
    }

    [Serializable]
    public class SettingsData
    {
        public List<SettingState> Int = new List<SettingState>();
        public List<SettingState> String = new List<SettingState>();
        public List<SettingState> Bool = new List<SettingState>();
        public List<SettingState> Float = new List<SettingState>();
    }

    public Data DataBase;

    public SettingsData Settings;

    public List<RouletteManager.Result> StreakData;

    string saveFileName = "/DBDRouletteSaveData.save";

    string streakfileName = "/DBDRouletteStreakData.save";

    string statsfileName = "/DBDRouletteStats.tsv";

    public Action OnUserDataSaved;
    public Action OnUserDataLoaded;

    public Action OnStreakDataSaved;
    public Action OnStreakDataLoaded;

    public Action<SettingsScriptable> OnSettingUpdated;

    [SerializeField] BoolData SaveStatsBool;

    List<string> streakContent = new List<string>();

    bool init;

#if UNITY_EDITOR

    [ContextMenu("Editor Load Database")]
    void LoadEditorDatabase()
    {
        DataBase = new Data();

        foreach (var item in AssetDatabase.FindAssets("t:Characters"))
        {
            string path = AssetDatabase.GUIDToAssetPath(item);

            Characters current = (Characters)AssetDatabase.LoadAssetAtPath(path, typeof(Characters));

            if (current.Type == Characters.CharacterType.Killers)
            {
                CheckDatabaseContain(current, DataBase.Killers);

                foreach (var addon in current.KillerAddons)
                {
                    CheckDatabaseContain(addon, DataBase.KillerAddons);
                }
            }
            else
            {
                CheckDatabaseContain(current, DataBase.Survivors);
            }
        }

        foreach (var item in AssetDatabase.FindAssets("t:Perks"))
        {
            string path = AssetDatabase.GUIDToAssetPath(item);

            Perks current = (Perks)AssetDatabase.LoadAssetAtPath(path, typeof(Perks));

            if (current.name == "dudPerk") continue;

            if (current.name[0] == 'K')
            {
                CheckDatabaseContain(current, DataBase.KillerPerks);
            }
            else if (current.name[0] == 'S')
            {
                CheckDatabaseContain(current, DataBase.SurvivorPerks);
            }
        }

        foreach (var item in AssetDatabase.FindAssets("t:Items"))
        {
            string path = AssetDatabase.GUIDToAssetPath(item);

            Items current = (Items)AssetDatabase.LoadAssetAtPath(path, typeof(Items));

            if (current.name == "dudItem") continue;

            CheckDatabaseContain(current, DataBase.Items);

            foreach (var addon in current.ItemAddons)
            {
                CheckDatabaseContain(addon, DataBase.ItemsAddon);
            }
        }
    }


    void CheckDatabaseContain(BaseScriptable scriptable, List<DataState> list)
    {
        foreach (var item in list)
        {
            if (item.Ref == scriptable)
            {
                return;
            }
        }

        DataState newData = new DataState();
        newData.Ref = scriptable;
        newData.State = true;

        list.Add(newData);
    }

#endif

    public void SaveUserData()
    {
        string save = Application.version + '\n';

        save = ConvertData(save, DataBase.Survivors);
        save += '\n';

        save = ConvertData(save, DataBase.SurvivorPerks);
        save += '\n';

        save = ConvertData(save, DataBase.Killers);
        save += '\n';

        save = ConvertData(save, DataBase.KillerPerks);
        save += '\n';

        save = ConvertData(save, DataBase.KillerAddons);
        save += '\n';

        save = ConvertData(save, DataBase.Items);
        save += '\n';

        save = ConvertData(save, DataBase.ItemsAddon);
        save += '\n';

        save = ConvertSettingsData(save, Settings.Int);
        save += '\n';

        save = ConvertSettingsData(save, Settings.String);
        save += '\n';

        save = ConvertSettingsData(save, Settings.Bool);
        save += '\n';

        save = ConvertSettingsData(save, Settings.Float);

        SaveData(save, Application.persistentDataPath + saveFileName);
        OnUserDataSaved?.Invoke();

        if (GetSetting(SaveStatsBool))
        {
            SaveStats();
        }
    }
    public void LoadUserData()
    {
        string save = LoadData(Application.persistentDataPath + saveFileName);

        if (save == "")
        {
            SaveUserData();
            LoadUserData();
            return;
        }

        string[] saveLines = save.Split('\n');

        if (!CheckVersion(saveLines[0]))
        { 
            SaveUserData();
            LoadUserData();
            return;
        }

        if (saveLines.Length != 12) return;

        ReconvertData(saveLines[1], DataBase.Survivors);
        ReconvertData(saveLines[2], DataBase.SurvivorPerks);

        ReconvertData(saveLines[3], DataBase.Killers);
        ReconvertData(saveLines[4], DataBase.KillerPerks);
        ReconvertData(saveLines[5], DataBase.KillerAddons);

        ReconvertData(saveLines[6], DataBase.Items);
        ReconvertData(saveLines[7], DataBase.ItemsAddon);

        ReconvertSettingsData(saveLines[8], Settings.Int, 0);
        ReconvertSettingsData(saveLines[9], Settings.String, 1);
        ReconvertSettingsData(saveLines[10], Settings.Bool, 2);
        ReconvertSettingsData(saveLines[11], Settings.Float, 3);

        OnUserDataLoaded?.Invoke();

        if (init && GetSetting(SaveStatsBool))
        {
            LoadStats();
        }

        init = true;
    }
    bool CheckVersion(string test)
    {
        string[] testSplit = test.Split('.');

        string[] versionSplit = Application.version.Split('.');

        int testNum;
        int versNum;

        if (int.TryParse(testSplit[1], out testNum))
        {
            if (int.TryParse(versionSplit[1], out versNum))
            {
                if (testNum < versNum)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            return false;
        }

        return false;
    }


    public void SaveStreakData()
    {
        string save = Application.version + '\n';

        foreach (var item in streakContent)
        {
            save += item + '\n';
        }

        SaveData(save, Application.persistentDataPath + streakfileName);
        OnStreakDataSaved?.Invoke();
    }
    public void LoadStreakData()
    {
        StreakData = new List<RouletteManager.Result>();

        string save = LoadData(Application.persistentDataPath + streakfileName);

        if (save == "") return;

        string[] saveLines = save.Split('\n');

        if (!CheckVersion(saveLines[0]))
        {
            Debug.LogError("Can't read streak data");
            return;
        }

        for (int i = 1; i < saveLines.Length; i++)
        {
            if(saveLines[i] != "") AddNewEntry(ReadEntry(saveLines[i]));
        }

        OnStreakDataLoaded?.Invoke();

        RouletteManager.Instance.StreakOnGoing = true;
        RouletteManager.Instance.SetCurrentList(true);
        RouletteManager.Instance.ForceRoll();
    }

    public void ResetStreak()
    {
        StreakData = new List<RouletteManager.Result>();
        streakContent = new List<string>();
    }
    public void AddNewEntry(RouletteManager.Result result)
    {
        StreakData.Add(result);

        string currentResult = "";

        currentResult += "c:" + result.Character.ID + "*";

        currentResult += "p:";

        foreach (var item in result.Perks)
        {
            if (item != RouletteManager.Instance.Duds.PerkDud)
            {
                currentResult += item.name + "-";
            }
        }

        currentResult += "*a:";

        foreach (var item in result.Addons)
        {
            if (item != RouletteManager.Instance.Duds.AddonDud)
            {
                currentResult += item.name + "-";
            }
        }

        currentResult += "*i:";

        if (result.Item != null) currentResult += result.Item.name;

        currentResult += "*t:" + result.Roll.ToString();

        streakContent.Add(currentResult);
    }
    RouletteManager.Result ReadEntry(string s)
    {
        RouletteManager.Result result = new RouletteManager.Result();

        string[] starPart = s.Split('*');

        string Name = starPart[0].Substring(2);
        string[] Perks = starPart[1].Substring(2).Split('-');
        string[] Addons = starPart[2].Substring(2).Split('-');
        string Item = starPart[3].Substring(2);
        string Type = starPart[4].Substring(2);

        if (Type[0] == 'B')
        {
            result.Roll = RouletteManager.MainRollType.Both;
        }
        else if (Type[0] == 'K')
        {
            result.Roll = RouletteManager.MainRollType.Killer;
        }
        else if (Type[0] == 'S')
        {
            result.Roll = RouletteManager.MainRollType.Survivor;
        }

        if (Name[0] == 'K')
        {
            foreach (var item in DataBase.Killers)
            {
                if (item.Ref.name == Name)
                {
                    result.Character = item.Ref as Characters;
                    break;
                }
            }

            foreach (var perk in Perks)
            {
                foreach (var item in DataBase.KillerPerks)
                {
                    if(item.Ref.name == perk)
                    {
                        result.Perks.Add(item.Ref as Perks);
                        break;
                    }
                }
            }

            foreach (var addon in Addons)
            {
                foreach (var item in DataBase.KillerAddons)
                {
                    if (item.Ref.name == addon)
                    {
                        result.Addons.Add(item.Ref as Addons);
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (var item in DataBase.Survivors)
            {
                if (item.Ref.name == Name)
                {
                    result.Character = item.Ref as Characters;
                    break;
                }
            }

            foreach (var perk in Perks)
            {
                foreach (var item in DataBase.SurvivorPerks)
                {
                    if (item.Ref.name == perk)
                    {
                        result.Perks.Add(item.Ref as Perks);
                        break;
                    }
                }
            }

            foreach (var item in DataBase.Items)
            {
                if (item.Ref.name == Item)
                {
                    result.Item = item.Ref as Items;
                    break;
                }
            }

            if (result.Item != null)
            {
                foreach (var addon in Addons)
                {
                    foreach (var item in DataBase.ItemsAddon)
                    {
                        if (item.Ref.name == addon)
                        {
                            result.Addons.Add(item.Ref as Addons);
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }


    string ConvertData(string save, List<DataState> list)
    {
        foreach (var item in list)
        {
            SavedBoolData newData = new SavedBoolData();
            newData.Key = item.Ref.name;
            newData.Value = item.State;

            save += JsonUtility.ToJson(newData);
            save += '\t';
        }

        return save;
    }
    void ReconvertData(string save, List<DataState> list)
    {
        string[] saveSplit = save.Split('\t');

        foreach (var item in saveSplit)
        {
            SavedBoolData data = JsonUtility.FromJson<SavedBoolData>(item);

            if (data != null)
            {
                foreach (var countainedItem in list)
                {
                    if (countainedItem.Ref.name == data.Key)
                    {
                        countainedItem.State = data.Value;
                        break;
                    }
                }
            }
        }
    }


    string ConvertSettingsData(string save, List<SettingState> list)
    {
        foreach (var item in list)
        {
            if (item.Ref is IntData)
            {
                SavedIntData newData = new SavedIntData();
                newData.Key = item.Ref.name;
                newData.Value = item.IntValue;

                save += JsonUtility.ToJson(newData);
                save += '\t';
            }

            if (item.Ref is StringData)
            {
                SavedStringData newData = new SavedStringData();
                newData.Key = item.Ref.name;
                newData.Value = item.StringValue;

                save += JsonUtility.ToJson(newData);
                save += '\t';
            }

            if (item.Ref is BoolData)
            {
                SavedBoolData newData = new SavedBoolData();
                newData.Key = item.Ref.name;
                newData.Value = item.BoolValue;

                save += JsonUtility.ToJson(newData);
                save += '\t';
            }

            if (item.Ref is FloatData)
            {
                SavedFloatData newData = new SavedFloatData();
                newData.Key = item.Ref.name;
                newData.Value = item.FloatValue;

                save += JsonUtility.ToJson(newData);
                save += '\t';
            }
        }

        return save;
    }
    void ReconvertSettingsData(string save, List<SettingState> list, int index)
    {
        string[] saveSplit = save.Split('\t');

        foreach (var item in saveSplit)
        {
            switch (index)
            {
                case 0:

                    SavedIntData dataInt = JsonUtility.FromJson<SavedIntData>(item);

                    if (dataInt != null)
                    {
                        foreach (var countainedItem in list)
                        {
                            if (countainedItem.Ref.name == dataInt.Key)
                            {
                                countainedItem.IntValue = dataInt.Value;
                                break;
                            }
                        }
                    }

                    break;

                case 1:

                    SavedStringData dataString = JsonUtility.FromJson<SavedStringData>(item);

                    if (dataString != null)
                    {
                        foreach (var countainedItem in list)
                        {
                            if (countainedItem.Ref.name == dataString.Key)
                            {
                                countainedItem.StringValue = dataString.Value;
                                break;
                            }
                        }
                    }

                    break;

                case 2:

                    SavedBoolData dataBool = JsonUtility.FromJson<SavedBoolData>(item);

                    if (dataBool != null)
                    {
                        foreach (var countainedItem in list)
                        {
                            if (countainedItem.Ref.name == dataBool.Key)
                            {
                                countainedItem.BoolValue = dataBool.Value;
                                break;
                            }
                        }
                    }

                    break;

                case 3:

                    SavedFloatData dataFloat = JsonUtility.FromJson<SavedFloatData>(item);

                    if (dataFloat != null)
                    {
                        foreach (var countainedItem in list)
                        {
                            if (countainedItem.Ref.name == dataFloat.Key)
                            {
                                countainedItem.FloatValue = dataFloat.Value;
                                break;
                            }
                        }
                    }


                    break;

                default:
                    break;
            }
        }
    }

    [ContextMenu("SaveStats")]
    public void SaveStats()
    {
        StatsManager.StatsTracked stat = StatsManager.Instance.Stats;

        string save = "Version : " + Application.version + '\n';

        save += 
            "rollMade\t" +

            "survivor\t" +
            "killers\t" +
            "survivorPercent\t" +
            "killerPercent\t" +

            "dudPerks\t" +
            "dudAddons\t" +
            "dudItems\t" +
            "dudPerksPercent\t" +
            "dudAddonsPercent\t" +
            "dudItemsPercent\t" +

            "commonCount\t" +
            "uncommonCount\t" +
            "rareCount\t" +
            "veryRareCount\t" +
            "ultraRareCount\t" +
            "commonPercent\t" +
            "uncommonPercent\t" +
            "rarePercent\t" +
            "veryRarePercent\t" +
            "ultraRarePercent\t" + 

            '\n';

        save += 
            stat.rollMade.ToString() + '\t' +

            stat.survivors.ToString() + '\t' +
            stat.killers.ToString() + '\t' +
            stat.survivorPercent.ToString() + '\t' +
            stat.killerPercent.ToString() + '\t' +

            stat.dudPerks.ToString() + '\t' +
            stat.dudAddons.ToString() + '\t' +
            stat.dudItems.ToString() + '\t' +
            stat.dudPerksPercent.ToString() + '\t' +
            stat.dudAddonsPercent.ToString() + '\t' +
            stat.dudItemsPercent.ToString() + '\t' +

            stat.commonCount.ToString() + '\t' +
            stat.uncommonCount.ToString() + '\t' +
            stat.rareCount.ToString() + '\t' +
            stat.veryRareCount.ToString() + '\t' +
            stat.ultraRareCount.ToString() + '\t' +
            stat.commonPercent.ToString() + '\t' +
            stat.uncommonPercent.ToString() + '\t' +
            stat.rarePercent.ToString() + '\t' +
            stat.veryRarePercent.ToString() + '\t' +
            stat.ultraRarePercent.ToString() + '\n';

        save += '\n';

        save += "Survivor Victories\tKiller Victories\tSurvivor Loses\tKiller Loses\n";

        save +=
            StatsManager.Instance.Items.SurvivorVictory.ToString() + '\t' +
            StatsManager.Instance.Items.KillerVictory.ToString() + '\t' +
            StatsManager.Instance.Items.SurvivorLose.ToString() + '\t' +
            StatsManager.Instance.Items.KillerLose.ToString() + '\t' + '\n';

        save += '\n';

        save += "Name\tID\tCounted\tVictory\tLose\n";

        foreach (var item in StatsManager.Instance.Items.ItemsCounted)
        {
            string line = "";

            line +=
                item.Name + '\t' +
                item.Ref.name + '\t' +
                item.Count.ToString() + '\t' +
                item.Victory.ToString() + '\t' +
                item.Lose.ToString() + '\t';

            line += '\n';
            save += line;
        }

        SaveData(save, Application.persistentDataPath + statsfileName);
    }
    [ContextMenu("LoadStats")]
    public void LoadStats()
    {
        string save = LoadData(Application.persistentDataPath + statsfileName);

        if (save == "")
        {
            Debug.LogError("Can't read Stats data");
            return; 
        }

        string[] saveLines = save.Split('\n');

        string version = saveLines[0];
        version = version.Replace("Version : ", "");

        if (!CheckVersion(version))
        {
            Debug.LogError("Can't read Stats data");
            return;
        }

        StatsManager.StatsTracked stat = new StatsManager.StatsTracked();

        string[] stats = saveLines[2].Split('\t');

        if (stats.Length == 21)
        {
            stat.rollMade = int.TryParse(stats[0], out stat.rollMade) ? stat.rollMade : 0;

            stat.survivors = int.TryParse(stats[0], out stat.survivors) ? stat.survivors : 0;
            stat.killers = int.TryParse(stats[1], out stat.killers) ? stat.killers : 0;
            stat.survivorPercent = float.TryParse(stats[2], out stat.survivorPercent) ? stat.survivorPercent : 0f;
            stat.killerPercent = float.TryParse(stats[3], out stat.killerPercent) ? stat.killerPercent : 0f;

            stat.dudPerks = int.TryParse(stats[4], out stat.dudPerks) ? stat.dudPerks : 0;
            stat.dudAddons = int.TryParse(stats[5], out stat.dudAddons) ? stat.dudAddons : 0;
            stat.dudItems = int.TryParse(stats[6], out stat.dudItems) ? stat.dudItems : 0;
            stat.dudPerksPercent = float.TryParse(stats[7], out stat.dudPerksPercent) ? stat.dudPerksPercent : 0f;
            stat.dudAddonsPercent = float.TryParse(stats[8], out stat.dudAddonsPercent) ? stat.dudAddonsPercent : 0f;
            stat.dudItemsPercent = float.TryParse(stats[9], out stat.dudItemsPercent) ? stat.dudItemsPercent : 0f;

            stat.commonCount = int.TryParse(stats[10], out stat.commonCount) ? stat.commonCount : 0;
            stat.uncommonCount = int.TryParse(stats[11], out stat.uncommonCount) ? stat.uncommonCount : 0;
            stat.rareCount = int.TryParse(stats[12], out stat.rareCount) ? stat.rareCount : 0;
            stat.veryRareCount = int.TryParse(stats[13], out stat.veryRareCount) ? stat.veryRareCount : 0;
            stat.ultraRareCount = int.TryParse(stats[14], out stat.ultraRareCount) ? stat.ultraRareCount : 0;
            stat.commonPercent = float.TryParse(stats[15], out stat.commonPercent) ? stat.commonPercent : 0;
            stat.uncommonPercent = float.TryParse(stats[16], out stat.uncommonPercent) ? stat.uncommonPercent : 0;
            stat.rarePercent = float.TryParse(stats[17], out stat.rarePercent) ? stat.rarePercent : 0;
            stat.veryRarePercent = float.TryParse(stats[18], out stat.veryRarePercent) ? stat.veryRarePercent : 0;
            stat.ultraRarePercent = float.TryParse(stats[19], out stat.ultraRarePercent) ? stat.ultraRarePercent : 0;
        }

        StatsManager.ItemCounter Item = new StatsManager.ItemCounter();

        string[] data = saveLines[5].Split('\t');

        if (data.Length == 5)
        {
            Item.SurvivorVictory = int.TryParse(stats[0], out Item.SurvivorVictory) ? Item.SurvivorVictory : 0;
            Item.KillerVictory = int.TryParse(stats[1], out Item.KillerVictory) ? Item.KillerVictory : 0;
            Item.SurvivorLose = int.TryParse(stats[2], out Item.SurvivorLose) ? Item.SurvivorLose : 0;
            Item.KillerLose = int.TryParse(stats[3], out Item.KillerLose) ? Item.KillerLose : 0;
        }
        

        for (int i = 8; i < saveLines.Length; i++)
        {
            if (saveLines[i] == "") continue;

            string[] tab = saveLines[i].Split('\t');

            BaseScriptable scriptable = GetScriptableFromName(tab[1]);

            if (scriptable != null)
            {
                StatsManager.BaseCounter newElement = new StatsManager.BaseCounter();

                newElement.Name = tab[0];

                newElement.Ref = scriptable;

                newElement.Count = int.TryParse(tab[2], out newElement.Count) ? newElement.Count : 0;
                newElement.Victory = int.TryParse(tab[3], out newElement.Victory) ? newElement.Victory : 0;
                newElement.Lose = int.TryParse(tab[4], out newElement.Lose) ? newElement.Lose : 0;

                Item.ItemsCounted.Add(newElement);
            }
            else
            {
                Debug.LogError("Couldn't read line : " + i);
            }
        }

        StatsManager.Instance.Stats = stat;
        StatsManager.Instance.Items = Item;
        StatsManager.Instance.OnDataUpdated();
    }

    public void UpdateSetting(IntData scriptable, int value)
    {
        foreach (var item in Settings.Int)
        {
            if (item.Ref == scriptable)
            {
                item.IntValue = value;
                OnSettingUpdated?.Invoke(item.Ref);
                break;
            }
        }
    }
    public void UpdateSetting(StringData scriptable, string value)
    {
        foreach (var item in Settings.String)
        {
            if (item.Ref == scriptable)
            {
                item.StringValue = value;
                OnSettingUpdated?.Invoke(item.Ref);
                break;
            }
        }
    }
    public void UpdateSetting(BoolData scriptable, bool value)
    {
        foreach (var item in Settings.Bool)
        {
            if (item.Ref == scriptable)
            {
                item.BoolValue = value;
                OnSettingUpdated?.Invoke(item.Ref);
                break;
            }
        }
    }
    public void UpdateSetting(FloatData scriptable, float value)
    {
        foreach (var item in Settings.Float)
        {
            if (item.Ref == scriptable)
            {
                item.FloatValue = value;
                OnSettingUpdated?.Invoke(item.Ref);
                break;
            }
        }
    }

    public int GetSetting(IntData scriptable)
    {
        foreach (var item in Settings.Int)
        {
            if (item.Ref == scriptable)
            {
                return item.IntValue;
            }
        }

        return -1;
    }
    public string GetSetting(StringData scriptable)
    {
        foreach (var item in Settings.String)
        {
            if (item.Ref == scriptable)
            {
                return item.StringValue;
            }
        }

        return "";
    }
    public bool GetSetting(BoolData scriptable)
    {
        foreach (var item in Settings.Bool)
        {
            if (item.Ref == scriptable)
            {
                return item.BoolValue;
            }
        }

        return false;
    }
    public float GetSetting(FloatData scriptable)
    {
        foreach (var item in Settings.Float)
        {
            if (item.Ref == scriptable)
            {
                return item.FloatValue;
            }
        }

        return -1f;
    }



    public void UpdateState(Characters scriptable, bool newState)
    {
        if (scriptable.name[0] == 'K')
        {
            foreach (var item in DataBase.Killers)
            {
                if (item.Ref == scriptable)
                {
                    item.State = newState;
                }
            }
        }
        else if (scriptable.name[0] == 'S')
        {
            foreach (var item in DataBase.Survivors)
            {
                if (item.Ref == scriptable)
                {
                    item.State = newState;
                }
            }
        }
    }
    public void UpdateState(Equipables scriptable, bool newState)
    {
        if (scriptable is Perks)
        {
            if (scriptable.name[0] == 'K')
            {
                foreach (var item in DataBase.KillerPerks)
                {
                    if (item.Ref == scriptable)
                    {
                        item.State = newState;
                    }
                }
            }
            else if (scriptable.name[0] == 'S')
            {
                foreach (var item in DataBase.SurvivorPerks)
                {
                    if (item.Ref == scriptable)
                    {
                        item.State = newState;
                    }
                }
            }
        }

        if (scriptable is Addons)
        {
            if (scriptable.name[0] == 'K')
            {
                foreach (var item in DataBase.KillerAddons)
                {
                    if (item.Ref == scriptable)
                    {
                        item.State = newState;
                    }
                }
            }
            else if (scriptable.name[0] == 'I')
            {
                foreach (var item in DataBase.ItemsAddon)
                {
                    if (item.Ref == scriptable)
                    {
                        item.State = newState;
                    }
                }
            }
        }
    }
    public void UpdateState(Items scriptable, bool newState)
    {
        foreach (var item in DataBase.Items)
        {
            if (item.Ref == scriptable)
            {
                item.State = newState;
            }
        }
    }

    public bool GetState(Characters scriptable)
    {
        if (scriptable.name[0] == 'K')
        {
            foreach (var item in DataBase.Killers)
            {
                if (item.Ref == scriptable)
                {
                    return item.State;
                }
            }
        }
        else if (scriptable.name[0] == 'S')
        {
            foreach (var item in DataBase.Survivors)
            {
                if (item.Ref == scriptable)
                {
                    return item.State;
                }
            }
        }

        return false;
    }
    public bool GetState(Equipables scriptable)
    {
        if(scriptable is Perks)
        {
            if (scriptable.name[0] == 'K')
            {
                foreach (var item in DataBase.KillerPerks)
                {
                    if (item.Ref == scriptable)
                    {
                        return item.State;
                    }
                }
            }
            else if (scriptable.name[0] == 'S')
            {
                foreach (var item in DataBase.SurvivorPerks)
                {
                    if (item.Ref == scriptable)
                    {
                        return item.State;
                    }
                }
            }
        }
        
        if (scriptable is Addons)
        {
            if (scriptable.name[0] == 'K')
            {
                foreach (var item in DataBase.KillerAddons)
                {
                    if (item.Ref == scriptable)
                    {
                        return item.State;
                    }
                }
            }
            else if (scriptable.name[0] == 'I')
            {
                foreach (var item in DataBase.ItemsAddon)
                {
                    if (item.Ref == scriptable)
                    {
                        return item.State;
                    }
                }
            }
        }

        return false;
    }
    public bool GetState(Items scriptable)
    {
        foreach (var item in DataBase.Items)
        {
            if (item.Ref == scriptable)
            {
                return item.State;
            }
        }

        return false;
    }


    public BaseScriptable GetScriptableFromName(string name)
    {
        if (name.Contains("SurvivorPerk"))
        {
            foreach (var item in DataBase.SurvivorPerks)
            {
                if (item.Ref.name == name)
                {
                    return item.Ref;
                }
            }
        }


        if (name.Contains("KillerPerk_"))
        {
            foreach (var item in DataBase.KillerPerks)
            {
                if (item.Ref.name == name)
                {
                    return item.Ref;
                }
            }
        }

        if (name.Contains("KillerAddon_"))
        {
            foreach (var item in DataBase.KillerAddons)
            {
                if (item.Ref.name == name)
                {
                    return item.Ref;
                }
            }
        }

        if (name.Contains("Item_"))
        {
            foreach (var item in DataBase.Items)
            {
                if (item.Ref.name == name)
                {
                    return item.Ref;
                }
            }
        }

        if (name.Contains("ItemAddon_"))
        {
            foreach (var item in DataBase.ItemsAddon)
            {
                if (item.Ref.name == name)
                {
                    return item.Ref;
                }
            }
        }

        foreach (var item in DataBase.Killers)
        {
            if (item.Ref.name == name)
            {
                return item.Ref;
            }
        }

        foreach (var item in DataBase.Survivors)
        {
            if (item.Ref.name == name)
            {
                return item.Ref;
            }
        }

        return null;
    }

    public void ResetData()
    {
        StatsManager.Instance.ResetStats();

        DataBase.Reset();

        foreach (var item in Settings.Bool)
        {
            item.Reset();
            OnSettingUpdated(item.Ref);
        }

        foreach (var item in Settings.Float)
        {
            item.Reset();
            OnSettingUpdated(item.Ref);
        }

        foreach (var item in Settings.Int)
        {
            item.Reset();
            OnSettingUpdated(item.Ref);
        }

        foreach (var item in Settings.String)
        {
            item.Reset();
            OnSettingUpdated(item.Ref);
        }
    }

    void SaveData(string data, string path)
    {
        File.WriteAllText(path, data);

        GUIUtility.systemCopyBuffer = path;
    }
    string LoadData(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return "";
    }
}
