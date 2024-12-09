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

    string saveFileName = "/DBDRouletteSaveData.save";

    string streakfileName = "/DBDRouletteStreakData.save";

    public Action OnUserDataSaved;
    public Action OnUserDataLoaded;

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


    public void UpdateSetting(IntData scriptable, int value)
    {
        foreach (var item in Settings.Int)
        {
            if (item.Ref == scriptable)
            {
                item.IntValue = value;
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
