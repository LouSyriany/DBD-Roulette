using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterUnlockedManager : MonoBehaviour
{
    /*
    [Serializable]
    class SavedEquipable
    {
        public Equipables Equipable;

        public bool State = true;
    }

    [Serializable]
    class ItemSaved
    {
        public Items Item;
        public bool ItemState;
    }

    [Serializable]
    class SavedCharacterUnlockedData
    {
        public Characters Character;
        public bool CharacterState = true;
        public List<SavedEquipable> Perks = new List<SavedEquipable>();
        public List<SavedEquipable> Addons = new List<SavedEquipable>();
    }

    [Serializable]
    class SavedItemUnlockedData
    {
        public List<ItemSaved> ItemSaved;
        public List<SavedEquipable> Addons = new List<SavedEquipable>();
    }

    [SerializeField] List<CharacterUnlockedItem> CharacterUnlockedItems = new List<CharacterUnlockedItem>();

    [SerializeField] List<ItemUnlocked> ItemsUnlocked = new List<ItemUnlocked>();

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

    void Start()
    {
        LoadData();
    }

    [ContextMenu("Save")]
    public void SaveData()
    {
        string save = "";

        for (int i = 0; i < CharacterUnlockedItems.Count; i++)
        {
            SavedCharacterUnlockedData data = new SavedCharacterUnlockedData();

            data.Character = CharacterUnlockedItems[i].Character;
            data.CharacterState = CharacterUnlockedItems[i].CharacterState;

            foreach (var perk in CharacterUnlockedItems[i].Perks)
            {
                if (perk.AddonSlot == null) continue;

                SavedEquipable nperk = new SavedEquipable();
                nperk.Equipable = perk.AddonSlot.Equipable;
                nperk.State = perk.State;
                data.Perks.Add(nperk);
            }

            foreach (var addon in CharacterUnlockedItems[i].Addons)
            {
                if (addon.AddonSlot == null) continue;

                SavedEquipable naddon = new SavedEquipable();
                naddon.Equipable = addon.AddonSlot.Equipable;
                naddon.State = addon.State;
                data.Addons.Add(naddon);
            }

            save += JsonUtility.ToJson(data);
            save += '\n';
        }

        save += "--Separator--";

        for (int i = 0; i < ItemsUnlocked.Count; i++)
        {
            SavedItemUnlockedData data = new SavedItemUnlockedData();

            foreach (var item in ItemsUnlocked[i].Items)
            {
                if (item.ItemSlot == null) continue;

                ItemSaved nitem = new ItemSaved();

                nitem.Item = item.ItemSlot.Item;
                nitem.ItemState = item.State;

                data.ItemSaved.Add(nitem);
            }

            foreach (var addon in ItemsUnlocked[i].Addons)
            {
                if (addon.AddonSlot == null) continue;
                SavedEquipable naddon = new SavedEquipable();

                naddon.Equipable = addon.AddonSlot.Equipable;
                naddon.State = addon.State;

                data.Addons.Add(naddon);
            }

            save += JsonUtility.ToJson(data);
            save += '\n';
        }

        DatasManager.Instance.SaveData(save);
    }

    public void LoadData()
    {
        string save = DatasManager.Instance.LoadData();

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
                if (data.Character == character.Character)
                {
                    character.CharacterState = data.CharacterState;

                    foreach (var perk in character.Perks)
                    {
                        foreach (var item in data.Perks)
                        {
                            if (item.Equipable == perk.AddonSlot.Equipable)
                            {
                                perk.State = item.State;
                            }
                        }
                    }

                    foreach (var addon in character.Addons)
                    {
                        foreach (var item in data.Addons)
                        {
                            if (item.Equipable == addon.AddonSlot.Equipable)
                            {
                                addon.State = item.State;
                            }
                        }
                    }
                }

                character.Setup();
            }
        }

        #endregion

        #region Items

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
                        if (itemSaved.Item == item.ItemSlot.Item)
                        {
                            item.State = itemSaved.ItemState;
                        }
                    }
                }

                foreach (var addonSaved in data.Addons)
                {
                    foreach (var addon in itemUnlocked.Addons)
                    {
                        if (addonSaved.Equipable == addon.AddonSlot.Equipable)
                        {
                            addon.State = addonSaved.State;
                        }
                    }
                }

                itemUnlocked.CheckState();
            }
        }

        #endregion
    }
    */
}
