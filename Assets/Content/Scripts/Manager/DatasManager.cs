using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(1)]
public class DatasManager : MonoBehaviour
{
    public static DatasManager Instance { get; private set; }

    [Serializable]
    class SavedRollEntry
    {
        public string Character;
        public string Item;
        public List<string> Addons = new List<string>();
        public List<string> Perks = new List<string>();
    }

    [Serializable]
    class SavedEquipable
    {
        public string Equipable;

        public bool State = true;
    }

    [Serializable]
    class ItemSaved
    {
        public string Item;
        public bool ItemState;
    }

    [Serializable]
    class SavedCharacterUnlockedData
    {
        public string Character;
        public bool CharacterState = true;
        public List<SavedEquipable> Perks = new List<SavedEquipable>();
        public List<SavedEquipable> Addons = new List<SavedEquipable>();
    }

    [Serializable]
    class SavedItemUnlockedData
    {
        public List<ItemSaved> ItemSaved = new List<ItemSaved>();
        public List<SavedEquipable> Addons = new List<SavedEquipable>();
    }

    [Serializable]
    class SavedSettingsBool
    {
        public SettingsManager.SettingBoolType Type;
        public bool Value;
    }

    [Serializable]
    class SavedSettingsInt
    {
        public SettingsManager.SettingIntType Type;
        public int Value;
    }

    [Serializable]
    class SavedSettings
    {
        public List<SavedSettingsBool> Bools = new List<SavedSettingsBool>();
        public List<SavedSettingsInt> Ints = new List<SavedSettingsInt>();
    }

    [SerializeField] List<CharacterUnlockedItem> CharacterUnlockedItems = new List<CharacterUnlockedItem>();

    [SerializeField] List<ItemUnlocked> ItemsUnlocked = new List<ItemUnlocked>();

    [SerializeField] List<SavedRollEntry> SavedStreaks = new List<SavedRollEntry>();

    [SerializeField] SettingsManager settings;

    string fileName = "/DBDRouletteSaveData.save";

#if UNITY_EDITOR
    [ContextMenu("Editor Load The Character Unlocked Items")]
    void LoadCharacterUnlocked()
    {
        CharacterUnlockedItems = new List<CharacterUnlockedItem>();

        foreach (CharacterUnlockedItem cui in Resources.FindObjectsOfTypeAll(typeof(CharacterUnlockedItem)) as CharacterUnlockedItem[])
        {
            if (!EditorUtility.IsPersistent(cui.transform.root.gameObject)
                && !(cui.hideFlags == HideFlags.NotEditable || cui.hideFlags == HideFlags.HideAndDontSave)
                && !CharacterUnlockedItems.Contains(cui))
                CharacterUnlockedItems.Add(cui);
        }

        ItemsUnlocked = new List<ItemUnlocked>();

        foreach (ItemUnlocked iu in Resources.FindObjectsOfTypeAll(typeof(ItemUnlocked)) as ItemUnlocked[])
        {
            if (!EditorUtility.IsPersistent(iu.transform.root.gameObject)
                && !(iu.hideFlags == HideFlags.NotEditable || iu.hideFlags == HideFlags.HideAndDontSave)
                && !ItemsUnlocked.Contains(iu))
                ItemsUnlocked.Add(iu);
        }
    }
#endif

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
    }

    void Start()
    {
        LoadUserData();
    }

    public void SaveUserData()
    {
        string save = "";

        #region Characters
        for (int i = 0; i < CharacterUnlockedItems.Count; i++)
        {
            SavedCharacterUnlockedData data = new SavedCharacterUnlockedData();

            data.Character = CharacterUnlockedItems[i].Character.name;
            data.CharacterState = CharacterUnlockedItems[i].CharacterState;

            foreach (var perk in CharacterUnlockedItems[i].Perks)
            {
                if (perk.AddonSlot == null) continue;

                SavedEquipable nperk = new SavedEquipable();
                nperk.Equipable = perk.AddonSlot.Equipable.name;
                nperk.State = perk.State;
                data.Perks.Add(nperk);
            }

            foreach (var addon in CharacterUnlockedItems[i].Addons)
            {
                if (addon.AddonSlot == null) continue;

                SavedEquipable naddon = new SavedEquipable();
                naddon.Equipable = addon.AddonSlot.Equipable.name;
                naddon.State = addon.State;
                data.Addons.Add(naddon);
            }

            save += JsonUtility.ToJson(data);
            save += '\n';
        }
        #endregion

        save += "--Separator--";

        #region Items

        for (int i = 0; i < ItemsUnlocked.Count; i++)
        {
            SavedItemUnlockedData data = new SavedItemUnlockedData();

            foreach (var item in ItemsUnlocked[i].Items)
            {
                if (item.ItemSlot == null) continue;

                ItemSaved nitem = new ItemSaved();

                nitem.Item = item.ItemSlot.Item.name;
                nitem.ItemState = item.State;

                data.ItemSaved.Add(nitem);
            }

            foreach (var addon in ItemsUnlocked[i].Addons)
            {
                if (addon.AddonSlot == null) continue;
                SavedEquipable naddon = new SavedEquipable();

                naddon.Equipable = addon.AddonSlot.Equipable.name;
                naddon.State = addon.State;

                data.Addons.Add(naddon);
            }

            save += JsonUtility.ToJson(data);
            save += '\n';
        }
        #endregion

        save += "--Separator--";

        #region Settings

        SavedSettings settingData = new SavedSettings();

        for (int i = 0; i < settings.SettingsBool.Count; i++)
        {
            SavedSettingsBool dataBool = new SavedSettingsBool();

            dataBool.Type = settings.SettingsBool[i].Setting;
            dataBool.Value = settings.SettingsBool[i].Toggle.isOn;

            settingData.Bools.Add(dataBool);
        }

        for (int i = 0; i < settings.SettingsInt.Count; i++)
        {
            SavedSettingsInt dataInt = new SavedSettingsInt();

            dataInt.Type = settings.SettingsInt[i].Setting;
            dataInt.Value = settings.SettingsInt[i].Slidder.currentValue;

            settingData.Ints.Add(dataInt);
        }

        save += JsonUtility.ToJson(settingData);
        save += '\n';

        #endregion

        SaveData(save);
    }

    public void LoadUserData()
    {
        string save = LoadData();

        if (save == "") return;

        string[] saves = save.Split("--Separator--");

        #region Characters

        string[] saveSplit = saves[0].Split('\n');

        List<SavedCharacterUnlockedData> retrievedData = new List<SavedCharacterUnlockedData>();

        foreach (string saveLine in saveSplit)
        {
            SavedCharacterUnlockedData data = JsonUtility.FromJson<SavedCharacterUnlockedData>(saveLine);

            if (data != null)
            {
                if (data.Character != null)
                {
                    retrievedData.Add(data);
                }
            }

        }

        foreach (SavedCharacterUnlockedData data in retrievedData)
        {
            foreach (var character in CharacterUnlockedItems)
            {
                if (data.Character == character.Character.name)
                {
                    character.CharacterState = data.CharacterState;

                    foreach (var perk in character.Perks)
                    {
                        foreach (var item in data.Perks)
                        {
                            if (item.Equipable == perk.AddonSlot.Equipable.name)
                            {
                                perk.State = item.State;
                            }
                        }
                    }

                    foreach (var addon in character.Addons)
                    {
                        foreach (var item in data.Addons)
                        {
                            try
                            {
                                if (item.Equipable == addon.AddonSlot.Equipable.name)
                                {
                                    addon.State = item.State;
                                }
                            }
                            catch (Exception)
                            {
                                Debug.Log(item.Equipable);
                                Debug.Log(addon);
                                throw;
                            }
                            
                        }
                    }
                }

                character.CheckState();
            }
        }

        #endregion

        #region Items

        if (saves.Length <= 1) return;

        saveSplit = saves[1].Split('\n');

        List<SavedItemUnlockedData> retrievedItemData = new List<SavedItemUnlockedData>();

        foreach (string saveLine in saveSplit)
        {
            SavedItemUnlockedData data = JsonUtility.FromJson<SavedItemUnlockedData>(saveLine);

            if (data != null)
            {
                if (data.ItemSaved.Count != 0)
                {
                    retrievedItemData.Add(data);
                }
            }

        }

        foreach (SavedItemUnlockedData data in retrievedItemData)
        {
            foreach (var itemUnlocked in ItemsUnlocked)
            {
                foreach (var itemSaved in data.ItemSaved)
                {
                    foreach (var item in itemUnlocked.Items)
                    {
                        if (itemSaved.Item == item.ItemSlot.Item.name)
                        {
                            item.State = itemSaved.ItemState;
                        }
                    }
                }

                foreach (var addonSaved in data.Addons)
                {
                    foreach (var addon in itemUnlocked.Addons)
                    {
                        if (addonSaved.Equipable == addon.AddonSlot.Equipable.name)
                        {
                            addon.State = addonSaved.State;
                        }
                    }
                }

                itemUnlocked.CheckState();
            }
        }

        #endregion

        #region Settings

        if (saves.Length <= 2) return;

        saveSplit = saves[2].Split('\n');

        SavedSettings retrievedSettingsData = new SavedSettings();

        foreach (string saveLine in saveSplit)
        {
            SavedSettings data = JsonUtility.FromJson<SavedSettings>(saveLine);

            if (data != null)
            {
                retrievedSettingsData = data;
            }
        }

        foreach (var nitem in retrievedSettingsData.Bools)
        {
            foreach (var item in settings.SettingsBool)
            {
                if (item.Setting == nitem.Type)
                {
                    item.Toggle.isOn = nitem.Value;
                }
            } 
        }

        foreach (var nitem in retrievedSettingsData.Ints)
        {
            foreach (var item in settings.SettingsInt)
            {
                if (item.Setting == nitem.Type)
                {
                    item.Slidder.SetValue(nitem.Value);
                }
            }
        }

        #endregion
    }

    void SaveData(string data)
    {
        File.WriteAllText(Application.persistentDataPath + fileName, data);

        GUIUtility.systemCopyBuffer = Application.persistentDataPath + fileName;
    }

    public void AddStreakEntries()
    {
        RouletteManager.Result result = RouletteManager.Instance.Results;

        SavedRollEntry newEntry = new SavedRollEntry();

        newEntry.Character = result.Character.ID;

        if (result.Item.name != "dudItem")
        {
            newEntry.Item = result.Item.name;
        }

        foreach (var addon in result.Addons)
        {
            if (addon.name != "dudAddon")
            {
                newEntry.Perks.Add(addon.name);
            }
        }

        foreach (var perk in result.Perks)
        {
            if(perk.name != "dudPerk")
            {
                newEntry.Perks.Add(perk.name);
            }
        }

        SavedStreaks.Add(newEntry);
        //Debug.Log(JsonUtility.ToJson(newEntry));
    }

    public void ResetStreakEntries()
    {
        SavedStreaks = new List<SavedRollEntry>();
    }

    string LoadData()
    {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            return File.ReadAllText(Application.persistentDataPath + fileName);
        }
        return "";
    }
}
