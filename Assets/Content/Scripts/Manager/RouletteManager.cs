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
        public bool RarirtyRoll = false;

        public bool CharacterStreakMode = false;
        public bool PerkStreakMode = false;

        [Space(5)]

        public bool DudPerk = true;
        public bool DudItem = true;
        public bool DudAddon = true;

        [Space(5)]

        public int DudPerkCount = 50;
        public int DudItemCount = 50;
        public int DudAddonCount = 50;

        [Space(5)]

        public bool RollCharacters = true;
        public bool RollAddons = true;
        public bool RollItems = true;
        public bool RollPerks = true;
    }

    [Serializable]
    public class Result
    {
        public Characters Character;

        public Items Item;

        public List<Perks> Perks = new List<Perks>();

        public List<Addons> Addons = new List<Addons>();
    }

    [SerializeField] RouletteData Data;

    public Dud Duds;

    public EnabledData EnabledDatas;

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

    [ContextMenu("Editor Setup Lists")]
    void EditorSetupLists()
    {
        List<EnabledCharacter> chara = new List<EnabledCharacter>();

        foreach (var item in Data.Survivors)
        {
            EnabledCharacter nchara = new EnabledCharacter();

            nchara.Enabled = true;
            nchara.Character = item;
            nchara.Name = item.name;

            chara.Add(nchara);
        }

        EnabledDatas.EnabledSurvivors = chara;


        chara = new List<EnabledCharacter>();

        foreach (var item in Data.Killers)
        {
            EnabledCharacter nchara = new EnabledCharacter();

            nchara.Enabled = true;
            nchara.Character = item;
            nchara.Name = item.name;

            nchara.EnabledKillerAddons = new List<EnabledAddons>();

            foreach (var addon in item.KillerAddons)
            {
                EnabledAddons naddon = new EnabledAddons();

                naddon.Addon = addon;

                nchara.EnabledKillerAddons.Add(naddon);
            }

            chara.Add(nchara);
        }

        EnabledDatas.EnabledKillers = chara;

        List<EnabledPerk> perks = new List<EnabledPerk>();

        foreach (var item in Data.SurvivorPerks)
        {
            EnabledPerk nperk = new EnabledPerk();

            nperk.Enabled = true;
            nperk.Perk = item;
            nperk.Name = item.name;

            perks.Add(nperk);
        }

        EnabledDatas.EnabledSurvivorPerks = perks;


        perks = new List<EnabledPerk>();

        foreach (var item in Data.KillerPerks)
        {
            EnabledPerk nperk = new EnabledPerk();

            nperk.Enabled = true;
            nperk.Perk = item;
            nperk.Name = item.name;

            perks.Add(nperk);
        }

        EnabledDatas.EnabledKillerPerks = perks;


        List<EnabledItem> items = new List<EnabledItem>();

        foreach (var item in Data.Items)
        {
            EnabledItem nitem = new EnabledItem();

            nitem.Enabled = true;
            nitem.Item = item;
            nitem.Name = item.name;

            nitem.EnabledItemAddons = new List<EnabledAddons>();

            foreach (var addon in item.ItemAddons)
            {
                EnabledAddons naddon = new EnabledAddons();

                naddon.Addon = addon;

                nitem.EnabledItemAddons.Add(naddon);
            }

            items.Add(nitem);
        }

        EnabledDatas.EnabledItems = items;
    }

    public void Roll(MainRollType rollType)
    {
        if (Parameters.CharacterStreakMode || Parameters.PerkStreakMode)
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

        RandomCharacter(rollType);

        if (Parameters.RollPerks) RandomPerks();

        if (Parameters.RollItems) RandomItem();

        if (Parameters.RollAddons) RandomAddons();


        if ((Parameters.CharacterStreakMode || Parameters.PerkStreakMode) && StreakOnGoing)
        {
            UpdateStreakList(rollType);
        }

        OnRollMade?.Invoke(Results);
    }
   

    void SetCurrentList()
    {
        CurrentList = new EnabledData();
        CurrentList.EnabledSurvivors = new List<EnabledCharacter>();
        CurrentList.EnabledSurvivorPerks = new List<EnabledPerk>();

        CurrentList.EnabledKillers = new List<EnabledCharacter>();
        CurrentList.EnabledKillerPerks = new List<EnabledPerk>();

        CurrentList.EnabledItems = new List<EnabledItem>();

        for (int i = 0; i < EnabledDatas.EnabledSurvivors.Count; i++)
        {
            EnabledCharacter chara = new EnabledCharacter();

            chara.Enabled = EnabledDatas.EnabledSurvivors[i].Enabled;
            chara.Character = EnabledDatas.EnabledSurvivors[i].Character;

            CurrentList.EnabledSurvivors.Add(chara);
        }

        for (int i = 0; i < EnabledDatas.EnabledKillers.Count; i++)
        {
            EnabledCharacter chara = new EnabledCharacter();

            chara.Enabled = EnabledDatas.EnabledKillers[i].Enabled;
            chara.Character = EnabledDatas.EnabledKillers[i].Character;

            for (int y = 0; y < EnabledDatas.EnabledKillers[i].EnabledKillerAddons.Count; y++)
            {
                EnabledAddons addon = new EnabledAddons();

                addon.Enabled = EnabledDatas.EnabledKillers[i].EnabledKillerAddons[y].Enabled;
                addon.Addon = EnabledDatas.EnabledKillers[i].EnabledKillerAddons[y].Addon;

                chara.EnabledKillerAddons.Add(addon);
            }

            CurrentList.EnabledKillers.Add(chara);
        }

        for (int i = 0; i < EnabledDatas.EnabledSurvivorPerks.Count; i++)
        {
            EnabledPerk perk = new EnabledPerk();

            perk.Enabled = EnabledDatas.EnabledSurvivorPerks[i].Enabled;
            perk.Perk = EnabledDatas.EnabledSurvivorPerks[i].Perk;

            CurrentList.EnabledSurvivorPerks.Add(perk);
        }

        for (int i = 0; i < EnabledDatas.EnabledKillerPerks.Count; i++)
        {
            EnabledPerk perk = new EnabledPerk();

            perk.Enabled = EnabledDatas.EnabledKillerPerks[i].Enabled;
            perk.Perk = EnabledDatas.EnabledKillerPerks[i].Perk;

            CurrentList.EnabledKillerPerks.Add(perk);
        }

        if (Parameters.PerkStreakMode)
        {
            AddStreakPerkDud(CurrentList.EnabledSurvivorPerks, false);
            AddStreakPerkDud(CurrentList.EnabledKillerPerks, true);
        }

        for (int i = 0; i < EnabledDatas.EnabledItems.Count; i++)
        {
            EnabledItem item = new EnabledItem();

            item.Enabled = EnabledDatas.EnabledItems[i].Enabled;
            item.Item = EnabledDatas.EnabledItems[i].Item;

            for (int y = 0; y < EnabledDatas.EnabledItems[i].EnabledItemAddons.Count; y++)
            {
                EnabledAddons addon = new EnabledAddons();

                addon.Enabled = EnabledDatas.EnabledItems[i].EnabledItemAddons[y].Enabled;
                addon.Addon = EnabledDatas.EnabledItems[i].EnabledItemAddons[y].Addon;

                item.EnabledItemAddons.Add(addon);
            }

            CurrentList.EnabledItems.Add(item);
        }
    }

    void RandomCharacter(MainRollType rollType)
    {
        List<Characters> CharacterToRoll = new List<Characters>();

        switch (rollType)
        {
            case MainRollType.Killer:

                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                break;

            case MainRollType.Survivor:

                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Enabled)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }

                break;

            case MainRollType.Both:

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

    public void RerollCharacter(Characters character)
    {
        List<Characters> CharacterToRoll = new List<Characters>();

        switch (character.Type)
        {
            case Characters.CharacterType.Killers:

                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                break;

            case Characters.CharacterType.Survivors:

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

        OnRollMade?.Invoke(Results);
    }

    void RandomPerks()
    {
        List<Perks> perkList = new List<Perks>();

        if (Results.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killerPerk in CurrentList.EnabledKillerPerks)
            {
                if (killerPerk.Enabled)
                {
                    perkList.Add(killerPerk.Perk);
                }
            }
        }
        else
        {
            foreach (var survivorPerk in CurrentList.EnabledSurvivorPerks)
            {
                if (survivorPerk.Enabled)
                {
                    perkList.Add(survivorPerk.Perk);
                }
            }
        }

        if (!Parameters.PerkStreakMode && Parameters.DudPerk)
        {
            int count = Parameters.DudPerkCount * perkList.Count / 100;

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

    void RandomItem()
    {
        if (Results.Character.Type != Characters.CharacterType.Survivors) return;

        List<Items> itemList = new List<Items>();

        foreach (var item in CurrentList.EnabledItems)
        {
            if (item.Enabled && item.Item != Duds.ItemDud)
            {
                if (Parameters.RarirtyRoll)
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

        if (Parameters.DudItem)
        {
            int count = Parameters.DudItemCount * itemList.Count / 100;

            for (int i = 0; i < count; i++)
            {
                itemList.Add(Duds.ItemDud);
            }
        }

        RandomRouletteFn.Shuffle(itemList);

        rdm = UnityEngine.Random.Range(0, itemList.Count);

        Results.Item = itemList[rdm];
    }

    public void RerollItem(Items item)
    {
        if (Results.Character.Type != Characters.CharacterType.Survivors) return;

        List<Items> itemList = new List<Items>();

        foreach (var itemlist in CurrentList.EnabledItems)
        {
            if (itemlist.Enabled && itemlist.Item != Duds.ItemDud && item != itemlist.Item)
            {
                if (Parameters.RarirtyRoll)
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
                            if (Parameters.RarirtyRoll)
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
                                if (Parameters.RarirtyRoll)
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
            if (Parameters.DudAddon)
            {
                int count = Parameters.DudAddonCount * addonList.Count / 100;

                for (int i = 0; i < count; i++)
                {
                    addonList.Add(Duds.AddonDud);
                }
            }

            RandomRouletteFn.Shuffle(addonList);
            if (Parameters.RarirtyRoll)
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
                            if (Parameters.RarirtyRoll)
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
                                if (Parameters.RarirtyRoll)
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
        if (Parameters.DudPerk)
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
            if (Parameters.CharacterStreakMode)
            {
                foreach (var killer in CurrentList.EnabledKillers)
                {
                    if (killer.Character == Results.Character)
                    {
                        killer.Enabled = false;
                    }
                }
            }

            if (Parameters.PerkStreakMode)
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
            if (Parameters.CharacterStreakMode)
            {
                foreach (var survivor in CurrentList.EnabledSurvivors)
                {
                    if (survivor.Character == Results.Character)
                    {
                        survivor.Enabled = false;
                    }
                }
            }

            if (Parameters.PerkStreakMode)
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

        if (Parameters.CharacterStreakMode)
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

        if (Parameters.PerkStreakMode)
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

    public void SetDudPerk(bool b) { Parameters.DudPerk = b; }
    public void SetDudAddon(bool b) { Parameters.DudAddon = b; }
    public void SetDudItem(bool b) { Parameters.DudItem = b; }

    public void SetDudPerkCount(int i) { Parameters.DudPerkCount = i; }
    public void SetDudAddonCount(int i) { Parameters.DudAddonCount = i; }
    public void SetDudItemCount(int i) { Parameters.DudItemCount = i; }

    public void SetRollCharacters(bool b) { Parameters.RollCharacters = b; }
    public void SetRollPerk(bool b) { Parameters.RollPerks = b; }
    public void SetRollAddon(bool b) { Parameters.RollAddons = b; }
    public void SetRollItem(bool b) { Parameters.RollItems = b; }
    public void SetRarityRoll(bool b) { Parameters.RarirtyRoll = b; }

    public void SetCharacterStreak(bool b) { Parameters.CharacterStreakMode = b; }
    public void SetPerkStreak(bool b) { Parameters.PerkStreakMode = b; }

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
