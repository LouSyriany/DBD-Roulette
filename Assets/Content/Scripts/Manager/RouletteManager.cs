using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class RouletteManager : MonoBehaviour
{
    public static RouletteManager Instance { get; private set; }

    [Serializable]
    public class EnabledCharacter
    {
        [HideInInspector] public string Name = "";
        public bool Enabled = true;
        public Characters Character;
        public List<EnabledAddons> EnabledKillerAddons = new List<EnabledAddons>();

        public int ActiveKillerAddons
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledKillerAddons)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }
    }

    [Serializable]
    public class EnabledPerk
    {
        [HideInInspector] public string Name = "";
        public bool Enabled = true;
        public Perks Perk;
    }

    [Serializable]
    public class EnabledItem
    {
        [HideInInspector] public string Name = "";
        public bool Enabled = true;
        public Items Item;
        public List<EnabledAddons> EnabledItemAddons = new List<EnabledAddons>();

        public int ActiveItemAddons
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledItemAddons)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }
    }

    [Serializable]
    public class EnabledAddons
    {
        public Addons Addon;
        public bool Enabled = true;
    }

    public enum MainRollType { Killer, Survivor, Both }

    [Serializable]
    public class RouletteData
    {
        public Characters[] Survivors;
        public Perks[] SurvivorPerks;

        public Characters[] Killers;
        public Perks[] KillerPerks;

        public Items[] Items;
    }

    [Serializable]
    public class Dud
    {
        public Addons AddonDud;
        public Items ItemDud;
        public Perks PerkDud;
    }

    [Serializable]
    public class EnabledData
    {
        public List<EnabledCharacter> EnabledSurvivors = new List<EnabledCharacter>();
        public List<EnabledPerk> EnabledSurvivorPerks = new List<EnabledPerk>();

        public List<EnabledCharacter> EnabledKillers = new List<EnabledCharacter>();
        public List<EnabledPerk> EnabledKillerPerks = new List<EnabledPerk>();

        public List<EnabledItem> EnabledItems = new List<EnabledItem>();

        public int ActiveSurvivors 
        {
            get 
            {
                int i = 0;

                foreach (var item in EnabledSurvivors)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }

        public int ActiveSurvivorsPerks
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledSurvivorPerks)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }

        public int ActiveKillers
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledKillers)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }
        public int ActiveKillersPerks
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledKillerPerks)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }
        public int ActiveItems
        {
            get
            {
                int i = 0;

                foreach (var item in EnabledItems)
                {
                    if (item.Enabled) i++;
                }

                return i;
            }
        }
    }

    [Serializable]
    public class Parameter
    {
        public BoolData RarirtyRoll;

        public BoolData CharacterStreakMode;
        public BoolData PerkStreakMode;

        [Space(5)]

        public BoolData DudPerk;
        public BoolData DudItem;
        public BoolData DudAddon;

        [Space(5)]

        public IntData DudPerkCount;
        public IntData DudItemCount;
        public IntData DudAddonCount;

        [Space(5)]

        public BoolData RollCharacters;
        public BoolData RollAddons;
        public BoolData RollItems;
        public BoolData RollPerks;
    }

    [Serializable]
    public class Result
    {
        public MainRollType Roll;

        public Characters Character;

        public Items Item;

        public List<Perks> Perks = new List<Perks>();

        public List<Addons> Addons = new List<Addons>();
    }

    //public RouletteData Data;

    public Dud Duds;

    //public EnabledData EnabledDatas;

    public Parameter Parameters;

    [ReadOnly] public EnabledData CurrentList;

    int rdm;

    public Result Results;

    public bool StreakOnGoing = false;

    public bool StreakStopped = false;

    public Action<Result> OnRollMade;

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

    void SetCurrentList()
    {
        DatasManagerV2 data = DatasManagerV2.Instance;

        CurrentList = new EnabledData();
        CurrentList.EnabledSurvivors = new List<EnabledCharacter>();
        CurrentList.EnabledSurvivorPerks = new List<EnabledPerk>();

        CurrentList.EnabledKillers = new List<EnabledCharacter>();
        CurrentList.EnabledKillerPerks = new List<EnabledPerk>();

        CurrentList.EnabledItems = new List<EnabledItem>();

        foreach (var item in data.DataBase.Survivors)
        {
            EnabledCharacter chara = new EnabledCharacter();
            chara.Enabled = item.State;
            chara.Character = item.Ref as Characters;

            CurrentList.EnabledSurvivors.Add(chara);
        }

        foreach (var item in data.DataBase.Killers)
        {
            EnabledCharacter chara = new EnabledCharacter();
            chara.Enabled = item.State;
            chara.Character = item.Ref as Characters;

            string name = chara.Character.ID;
            name = name.Replace("_", "");

            foreach (var addon in data.DataBase.KillerAddons)
            {
                if (addon.Ref.name.Contains(name))
                {
                    EnabledAddons newaddon = new EnabledAddons();

                    newaddon.Enabled = addon.State;
                    newaddon.Addon = addon.Ref as Addons;

                    chara.EnabledKillerAddons.Add(newaddon);
                }
            }

            CurrentList.EnabledKillers.Add(chara);
        }

        foreach (var item in data.DataBase.SurvivorPerks)
        {
            EnabledPerk perk = new EnabledPerk();

            perk.Enabled = item.State;
            perk.Perk = item.Ref as Perks;

            CurrentList.EnabledSurvivorPerks.Add(perk);
        }

        foreach (var item in data.DataBase.KillerPerks)
        {
            EnabledPerk perk = new EnabledPerk();

            perk.Enabled = item.State;
            perk.Perk = item.Ref as Perks;

            CurrentList.EnabledKillerPerks.Add(perk);
        }
        
        if (DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode))
        {
            AddStreakPerkDud(CurrentList.EnabledSurvivorPerks, false);
            AddStreakPerkDud(CurrentList.EnabledKillerPerks, true);
        }

        foreach (var item in data.DataBase.Items)
        {
            EnabledItem eItem = new EnabledItem();

            eItem.Enabled = item.State;
            eItem.Item = item.Ref as Items;

            string name = eItem.Item.Type.ToString();
            name = name.Replace(" ", "");

            foreach (var addon in data.DataBase.ItemsAddon)
            {
                if (addon.Ref.name.Contains(name))
                {
                    EnabledAddons newaddon = new EnabledAddons();

                    newaddon.Enabled = addon.State;
                    newaddon.Addon = addon.Ref as Addons;

                    eItem.EnabledItemAddons.Add(newaddon);
                }
            }

            CurrentList.EnabledItems.Add(eItem);
        }
    }

    public void RollSurvivor() { Roll(MainRollType.Survivor); }
    public void RollKiller() { Roll(MainRollType.Killer); }
    public void RollBoth() 
    {
        int result = UnityEngine.Random.Range(0, 2);

        if (result == 0)
        {
            Roll(MainRollType.Survivor);
        }
        else if (result == 1)
        {
            Roll(MainRollType.Killer);
        }
    }
    public void Roll(MainRollType rollType)
    {
        if (DatasManagerV2.Instance.GetSetting(Parameters.CharacterStreakMode)  || DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode))
        {
            if (!StreakOnGoing)
            {
                SetupStreak();
            }
        }
        else
        {
            SetCurrentList();
        }

        Results = new Result();

        Results.Roll = rollType;

        if (DatasManagerV2.Instance.GetSetting(Parameters.RollCharacters)) RandomCharacter(rollType);

        if (DatasManagerV2.Instance.GetSetting(Parameters.RollPerks)) RandomPerks(rollType);
        if (DatasManagerV2.Instance.GetSetting(Parameters.RollItems)) RandomItem(rollType);
        if (DatasManagerV2.Instance.GetSetting(Parameters.RollAddons)) RandomAddons();


        if ((DatasManagerV2.Instance.GetSetting(Parameters.CharacterStreakMode) || DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode)) && StreakOnGoing)
        {
            UpdateStreakList(rollType);

            //DatasManager.Instance.AddStreakEntries();
        }

        OnRollMade?.Invoke(Results);
    }

    

    void RandomCharacter(MainRollType rollType)
    {
        List<Characters> CharacterToRoll = new List<Characters>();

        switch (rollType)
        {
            case MainRollType.Killer:

                if (CurrentList.ActiveKillers == 0) return;

                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                break;

            case MainRollType.Survivor:

                if (CurrentList.ActiveSurvivors == 0) return;

                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Enabled)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }

                break;

            case MainRollType.Both:

                if (CurrentList.ActiveKillers == 0 && CurrentList.ActiveSurvivors == 0) return;

                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Enabled)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }
                break;
        }

        rdm = UnityEngine.Random.Range(0, CharacterToRoll.Count);

        Results.Character = CharacterToRoll[rdm];
    }
    void RandomPerks(MainRollType rollType)
    {
        List<Perks> perkList = new List<Perks>();

        if (rollType == MainRollType.Killer)
        {
            if (CurrentList.ActiveKillersPerks == 0) return;

            foreach (var killerPerk in CurrentList.EnabledKillerPerks)
            {
                if (killerPerk.Enabled)
                {
                    perkList.Add(killerPerk.Perk);
                }
            }
        }
        else if (rollType == MainRollType.Survivor)
        {
            if (CurrentList.ActiveSurvivorsPerks == 0) return;

            foreach (var survivorPerk in CurrentList.EnabledSurvivorPerks)
            {
                if (survivorPerk.Enabled)
                {
                    perkList.Add(survivorPerk.Perk);
                }
            }
        }

        if (!DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode) && DatasManagerV2.Instance.GetSetting(Parameters.DudPerk))
        {
            int count = DatasManagerV2.Instance.GetSetting(Parameters.DudPerkCount) * perkList.Count / 100;

            for (int i = 0; i < count; i++)
            {
                perkList.Add(Duds.PerkDud);
            }
        }

        RandomRouletteFn.Shuffle(perkList);

        if (perkList.Count > 0)
        {
            rdm = UnityEngine.Random.Range(0, perkList.Count);
            Results.Perks.Add(perkList[rdm]);
            perkList.RemoveAt(rdm);
        }

        if (perkList.Count > 0)
        {
            rdm = UnityEngine.Random.Range(0, perkList.Count);
            Results.Perks.Add(perkList[rdm]);
            perkList.RemoveAt(rdm);
        }

        if (perkList.Count > 0)
        {
            rdm = UnityEngine.Random.Range(0, perkList.Count);
            Results.Perks.Add(perkList[rdm]);
            perkList.RemoveAt(rdm);
        }

        if (perkList.Count > 0)
        {
            rdm = UnityEngine.Random.Range(0, perkList.Count);
            Results.Perks.Add(perkList[rdm]);
            perkList.RemoveAt(rdm);
        }
    }
    void RandomItem(MainRollType rollType)
    {
        if (rollType != MainRollType.Survivor) return;

        if (CurrentList.ActiveItems == 0) return;

        List<Items> itemList = new List<Items>();

        foreach (var item in CurrentList.EnabledItems)
        {
            if (item.Enabled && item.Item != Duds.ItemDud)
            {
                if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                {
                    for (int i = 0; i < 5 - (int)item.Item.Rarity; i++)
                    {
                        itemList.Add(item.Item);
                    }
                }
                else
                {
                    itemList.Add(item.Item);
                }
            }
        }

        if (DatasManagerV2.Instance.GetSetting(Parameters.DudItem))
        {
            int count = DatasManagerV2.Instance.GetSetting(Parameters.DudItemCount) * itemList.Count / 100;

            for (int i = 0; i < count; i++)
            {
                itemList.Add(Duds.ItemDud);
            }
        }

        RandomRouletteFn.Shuffle(itemList);

        rdm = UnityEngine.Random.Range(0, itemList.Count);

        Results.Item = itemList[rdm];
    }
    void RandomAddons()
    {
        List<Addons> addonList = new List<Addons>();

        if (Results.Character != null && Results.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killer in CurrentList.EnabledKillers)
            {
                if (killer.Character == Results.Character)
                {
                    foreach (var addon in killer.EnabledKillerAddons)
                    {
                        if (addon.Enabled && addon.Addon != Duds.AddonDud)
                        {
                            if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                            {
                                for (int i = 0; i < 5 - (int)addon.Addon.RarityType + 1; i++)
                                {
                                    addonList.Add(addon.Addon);
                                }
                            }
                            else
                            {
                                addonList.Add(addon.Addon);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Results.Item != null && Results.Item != Duds.ItemDud)
            {
                foreach (var item in CurrentList.EnabledItems)
                {
                    if (item.Item == Results.Item)
                    {
                        foreach (var addon in item.EnabledItemAddons)
                        {
                            if (addon.Enabled && addon.Addon != Duds.AddonDud)
                            {
                                if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                                {
                                    for (int i = 0; i < 5 - (int)addon.Addon.RarityType + 1; i++)
                                    {
                                        addonList.Add(addon.Addon);
                                    }
                                }
                                else
                                {
                                    addonList.Add(addon.Addon);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (addonList.Count > 0)
        {
            if (DatasManagerV2.Instance.GetSetting(Parameters.DudAddon))
            {
                int count = DatasManagerV2.Instance.GetSetting(Parameters.DudAddonCount) * addonList.Count / 100;

                for (int i = 0; i < count; i++)
                {
                    addonList.Add(Duds.AddonDud);
                }
            }

            RandomRouletteFn.Shuffle(addonList);
            if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
            {
                rdm = UnityEngine.Random.Range(0, addonList.Count);
                Results.Addons.Add(addonList[rdm]);

                Addons tmp = addonList[rdm];

                while (tmp == addonList[rdm])
                {
                    rdm = UnityEngine.Random.Range(0, addonList.Count);
                }

                Results.Addons.Add(addonList[rdm]);
            }
            else
            {
                rdm = UnityEngine.Random.Range(0, addonList.Count);
                Results.Addons.Add(addonList[rdm]);
                addonList.RemoveAt(rdm);

                rdm = UnityEngine.Random.Range(0, addonList.Count);
                Results.Addons.Add(addonList[rdm]);
                addonList.RemoveAt(rdm);
            }
        }
    }


    public void RerollCharacter(Characters character)
    {
        List<Characters> CharacterToRoll = new List<Characters>();

        switch (character.Type)
        {
            case Characters.CharacterType.Killers:

                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Enabled && killer.Character != character)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                break;

            case Characters.CharacterType.Survivors:

                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Enabled && survivor.Character != character)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }

                break;
        }

        rdm = UnityEngine.Random.Range(0, CharacterToRoll.Count);

        Results.Character = CharacterToRoll[rdm];

        if (character.Type == Characters.CharacterType.Killers)
        {
            Results.Addons = new List<Addons>();

            RandomAddons();
        }

        OnRollMade?.Invoke(Results);
    }
    public void RerollPerk(Perks perk)
    {
        List<Perks> perkList = new List<Perks>();

        if (Results.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killerPerk in CurrentList.EnabledKillerPerks)
            {
                if (killerPerk.Enabled && killerPerk.Perk != perk)
                {
                    perkList.Add(killerPerk.Perk);
                }
            }
        }
        else
        {
            foreach (var survivorPerk in CurrentList.EnabledSurvivorPerks)
            {
                if (survivorPerk.Enabled && survivorPerk.Perk != perk)
                {
                    perkList.Add(survivorPerk.Perk);
                }
            }
        }

        RandomRouletteFn.Shuffle(perkList);

        rdm = UnityEngine.Random.Range(0, perkList.Count);

        for (int i = 0; i < Results.Perks.Count; i++)
        {
            if (perk == Results.Perks[i])
            {
                while (Results.Perks.Contains(perkList[rdm]))
                {
                    rdm = UnityEngine.Random.Range(0, perkList.Count);
                }

                Results.Perks[i] = perkList[rdm];

                break;
            }
        }

        OnRollMade?.Invoke(Results);
    }
    public void RerollItem(Items item)
    {
        if (Results.Character.Type != Characters.CharacterType.Survivors) return;

        List<Items> itemList = new List<Items>();

        foreach (var itemlist in CurrentList.EnabledItems)
        {
            if (itemlist.Enabled && itemlist.Item != Duds.ItemDud && item != itemlist.Item)
            {
                if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                {
                    for (int i = 0; i < 5 - (int)itemlist.Item.Rarity; i++)
                    {
                        itemList.Add(itemlist.Item);
                    }
                }
                else
                {
                    itemList.Add(itemlist.Item);
                }
            }
        }

        RandomRouletteFn.Shuffle(itemList);

        rdm = UnityEngine.Random.Range(0, itemList.Count);

        Results.Item = itemList[rdm];

        Results.Addons = new List<Addons>();

        RandomAddons();

        OnRollMade?.Invoke(Results);
    }
    public void RerollAddon(Addons addon)
    {
        List<Addons> addonList = new List<Addons>();

        if (Results.Character != null && Results.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killer in CurrentList.EnabledKillers)
            {
                if (killer.Character == Results.Character)
                {
                    foreach (var kaddon in killer.EnabledKillerAddons)
                    {
                        if (kaddon.Enabled && kaddon.Addon != Duds.AddonDud && kaddon.Addon != addon)
                        {
                            if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                            {
                                for (int i = 0; i < 5 - (int)kaddon.Addon.RarityType + 1; i++)
                                {
                                    addonList.Add(kaddon.Addon);
                                }
                            }
                            else
                            {
                                addonList.Add(kaddon.Addon);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Results.Item != null && Results.Item != Duds.ItemDud)
            {
                foreach (var item in CurrentList.EnabledItems)
                {
                    if (item.Item == Results.Item)
                    {
                        foreach (var saddon in item.EnabledItemAddons)
                        {
                            if (saddon.Enabled && saddon.Addon != Duds.AddonDud && saddon.Addon != addon)
                            {
                                if (DatasManagerV2.Instance.GetSetting(Parameters.RarirtyRoll))
                                {
                                    for (int i = 0; i < 5 - (int)saddon.Addon.RarityType + 1; i++)
                                    {
                                        addonList.Add(saddon.Addon);
                                    }
                                }
                                else
                                {
                                    addonList.Add(saddon.Addon);
                                }
                            }
                        }
                    }
                }
            }
        }

        RandomRouletteFn.Shuffle(addonList);

        rdm = UnityEngine.Random.Range(0, addonList.Count);

        for (int i = 0; i < Results.Addons.Count; i++)
        {
            if (addon == Results.Addons[i])
            {
                while (Results.Addons.Contains(addonList[rdm]))
                {
                    rdm = UnityEngine.Random.Range(0, addonList.Count);
                }

                Results.Addons[i] = addonList[rdm];

                break;
            }
        }

        OnRollMade?.Invoke(Results);
    }


    void SetupStreak()
    {
        StreakOnGoing = true;
    
        SetCurrentList();
    }
    void AddStreakPerkDud(List<EnabledPerk> list, bool isKiller)
    {
        if (DatasManagerV2.Instance.GetSetting(Parameters.DudPerk))
        {
            int perkCount = isKiller ? CurrentList.ActiveKillersPerks : CurrentList.ActiveSurvivorsPerks;

            int neededSlot = (isKiller ? CurrentList.ActiveKillers : CurrentList.ActiveSurvivors) * 4;

            int countToAdd = neededSlot - perkCount;

            for (int i = 0; i < countToAdd; i++)
            {
                EnabledPerk perk = new EnabledPerk();

                perk.Enabled = true;
                perk.Perk = Duds.PerkDud;

                list.Add(perk);
            }
        }
    }
    void UpdateStreakList(MainRollType rollType)
    {
        if (Results.Character.Type == Characters.CharacterType.Killers)
        {
            if (DatasManagerV2.Instance.GetSetting(Parameters.CharacterStreakMode))
            {
                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Character == Results.Character)
                    {
                        killer.Enabled = false;
                    }
                }
            }

            if (DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode))
            {
                foreach (var perk in Results.Perks)
                {
                    bool found = false;

                    foreach (var eperk in CurrentList.EnabledKillerPerks)
                    {
                        if (!eperk.Enabled) continue;

                        if (eperk.Perk == perk && !found)
                        {
                            found = true;
                            eperk.Enabled = false;
                            continue;
                        }
                    }
                }
            }  
        }
        else
        {
            if (DatasManagerV2.Instance.GetSetting(Parameters.CharacterStreakMode))
            {
                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Character == Results.Character)
                    {
                        survivor.Enabled = false;
                    }
                }
            }

            if (DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode))
            {
                foreach (var perk in Results.Perks)
                {
                    bool found = false;

                    foreach (var eperk in CurrentList.EnabledSurvivorPerks)
                    {
                        if (!eperk.Enabled) continue;

                        if (eperk.Perk == perk && !found)
                        {
                            found = true;
                            eperk.Enabled = false;
                            continue;
                        }
                    }
                }
            }
        }
        //Debug.Log(currentList.ActiveKillers + " - " + currentList.ActiveKillersPerks + " - " + currentList.ActiveSurvivors + " - " + currentList.ActiveSurvivorsPerks);

        if (DatasManagerV2.Instance.GetSetting(Parameters.CharacterStreakMode))
        {
            switch (rollType)
            {
                case MainRollType.Killer:
                    if (CurrentList.ActiveKillers == 0) StreakStopped = true;
                    break;

                case MainRollType.Survivor:
                    if (CurrentList.ActiveSurvivors == 0) StreakStopped = true;
                    break;

                case MainRollType.Both:
                    if (CurrentList.ActiveKillers == 0 && CurrentList.ActiveSurvivors == 0) StreakStopped = true;
                    break;
            }
        }

        if (DatasManagerV2.Instance.GetSetting(Parameters.PerkStreakMode))
        {
            switch (rollType)
            {
                case MainRollType.Killer:
                    if (CurrentList.ActiveKillersPerks == 0) StreakStopped = true;
                    break;

                case MainRollType.Survivor:
                    if (CurrentList.ActiveSurvivorsPerks == 0) StreakStopped = true;
                    break;

                case MainRollType.Both:
                    if (CurrentList.ActiveKillersPerks == 0 && CurrentList.ActiveSurvivorsPerks == 0) StreakStopped = true;
                    break;
            }
        }
    }
    public void ResetStreak()
    {
        StreakOnGoing = false;
        StreakStopped = false;
    }
}

public static class RandomRouletteFn
{
    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
