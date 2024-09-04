using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RouletteManager : MonoBehaviour
{
    public static RouletteManager Instance { get; private set; }

    [Serializable]
    public class EnabledCharacter
    {
        [HideInInspector] public string Name;
        public bool Enabled;
        public Characters Character;
        public List<EnabledAddons> EnabledKillerAddons = new List<EnabledAddons>();
    }

    [Serializable]
    public class EnabledPerk
    {
        [HideInInspector] public string Name;
        public bool Enabled;
        public Perks Perk;
    }

    [Serializable]
    public class EnabledItem
    {
        [HideInInspector] public string Name;
        public bool Enabled;
        public Items Item;
        public List<EnabledAddons> EnabledItemAddons = new List<EnabledAddons>();
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
        public List<EnabledCharacter> EnabledSurvivors;
        public List<EnabledPerk> EnabledSurvivorPerks;

        public List<EnabledCharacter> EnabledKillers;
        public List<EnabledPerk> EnabledKillerPerks;

        public List<EnabledItem> EnabledItems;
    }

    [Serializable]
    public class Parameter
    {
        public bool RarirtyRoll = false;

        public bool StreakMode = false;

        [Space(5)]

        public bool DudPerk = true;
        public bool DudItem = true;
        public bool DudAddon = true;

        [Space(5)]

        public int DudPerkCount = 50;
        public int DudItemCount = 50;
        public int DudAddonCount = 50;

        [Space(5)]

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

    EnabledData currentList;

    int rdm;

    Result result;

    bool streakOnGoing = false;

    [HideInInspector] public int KCharRemaining;
    [HideInInspector] public int SCharRemaining;

    [HideInInspector] public int KPerkRemaining;
    [HideInInspector] public int SPerkRemaining;

    [HideInInspector] public int ItemRemaining;

    [HideInInspector] public int AddonRarityRemaining;



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

    public Result Roll(MainRollType rollType)
    {
        if (Parameters.StreakMode)
        {
            if (!streakOnGoing)
            {
                SetupStreak();
            }
        }
        else
        {
            currentList = EnabledDatas;
        }

        result = new Result();

        RandomCharacter(rollType);

        if (Parameters.RollPerks) RandomPerks();

        if (Parameters.RollItems) RandomItem();

        if (Parameters.RollAddons) RandomAddons();


        if (Parameters.StreakMode && streakOnGoing)
        {
            UpdateStreakList();
        }

        return result;
    }

    void RandomCharacter(MainRollType rollType)
    {
        List<Characters> CharacterToRoll = new List<Characters>();

        switch (rollType)
        {
            case MainRollType.Killer:

                foreach (var killer in currentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                break;

            case MainRollType.Survivor:

                foreach (var survivor in currentList.EnabledSurvivors)
                {
                    if (survivor.Enabled)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }

                break;

            case MainRollType.Both:

                foreach (var killer in currentList.EnabledKillers)
                {
                    if (killer.Enabled)
                    {
                        CharacterToRoll.Add(killer.Character);
                    }
                }

                foreach (var survivor in currentList.EnabledSurvivors)
                {
                    if (survivor.Enabled)
                    {
                        CharacterToRoll.Add(survivor.Character);
                    }
                }
                break;
        }

        rdm = UnityEngine.Random.Range(0, CharacterToRoll.Count);

        result.Character = CharacterToRoll[rdm];
    }

    void RandomPerks()
    {
        List<Perks> perkList = new List<Perks>();

        if (result.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killerPerk in currentList.EnabledKillerPerks)
            {
                if (killerPerk.Enabled && killerPerk.Perk != Duds.PerkDud)
                {
                    perkList.Add(killerPerk.Perk);
                }
            }
        }
        else
        {
            foreach (var survivorPerk in currentList.EnabledSurvivorPerks)
            {
                if (survivorPerk.Enabled && survivorPerk.Perk != Duds.PerkDud)
                {
                    perkList.Add(survivorPerk.Perk);
                }
            }
        }

        if (Parameters.DudPerk)
        {
            int count = Parameters.DudPerkCount * perkList.Count / 100;

            for (int i = 0; i < count; i++)
            {
                perkList.Add(Duds.PerkDud);
            }
        }

        RandomRouletteFn.Shuffle(perkList);

        rdm = UnityEngine.Random.Range(0, perkList.Count);
        result.Perks.Add(perkList[rdm]);
        perkList.RemoveAt(rdm);

        rdm = UnityEngine.Random.Range(0, perkList.Count);
        result.Perks.Add(perkList[rdm]);
        perkList.RemoveAt(rdm);

        rdm = UnityEngine.Random.Range(0, perkList.Count);
        result.Perks.Add(perkList[rdm]);
        perkList.RemoveAt(rdm);

        rdm = UnityEngine.Random.Range(0, perkList.Count);
        result.Perks.Add(perkList[rdm]);
        perkList.RemoveAt(rdm);
    }

    void RandomItem()
    {
        if (result.Character.Type != Characters.CharacterType.Survivors) return;

        List<Items> itemList = new List<Items>();

        foreach (var item in currentList.EnabledItems)
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

        result.Item = itemList[rdm];
    }

    void RandomAddons()
    {
        List<Addons> addonList = new List<Addons>();

        if (result.Character != null && result.Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killer in currentList.EnabledKillers)
            {
                if (killer.Character == result.Character)
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
            if (result.Item != null && result.Item != Duds.ItemDud)
            {
                foreach (var item in currentList.EnabledItems)
                {
                    if (item.Item == result.Item)
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
                result.Addons.Add(addonList[rdm]);

                Addons tmp = addonList[rdm];

                while (tmp == addonList[rdm])
                {
                    rdm = UnityEngine.Random.Range(0, addonList.Count);
                }

                result.Addons.Add(addonList[rdm]);
            }
            else
            {
                rdm = UnityEngine.Random.Range(0, addonList.Count);
                result.Addons.Add(addonList[rdm]);
                addonList.RemoveAt(rdm);

                rdm = UnityEngine.Random.Range(0, addonList.Count);
                result.Addons.Add(addonList[rdm]);
                addonList.RemoveAt(rdm);
            }
        }
    }

    void SetupStreak()
    {
        KCharRemaining = 0;
        SCharRemaining = 0;

        KPerkRemaining = 0;
        SPerkRemaining = 0;

        ItemRemaining = 0;

        AddonRarityRemaining = 0;

        streakOnGoing = true;
        currentList = EnabledDatas;

        for (int i = 0; i < currentList.EnabledKillers.Count; i++)
        {
            if (currentList.EnabledKillers[i].Enabled) KCharRemaining++;
        }

        for (int i = 0; i < currentList.EnabledSurvivors.Count; i++)
        {
            if (currentList.EnabledKillers[i].Enabled) SCharRemaining++;
        }
    }

    void UpdateStreakList()
    {
        if (result.Character.Type == Characters.CharacterType.Killers)
        {
            for (int i = currentList.EnabledKillers.Count - 1; i >= 0; i--)
            {
                if (currentList.EnabledKillers[i].Character == result.Character) currentList.EnabledKillers.RemoveAt(i);
            }
        }
        else
        {

        }
    }

    public void SetDudPerk(bool b) { Parameters.DudPerk = b; }
    public void SetDudAddon(bool b) { Parameters.DudAddon = b; }
    public void SetDudItem(bool b) { Parameters.DudItem = b; }

    public void SetDudPerkCount(int i) { Parameters.DudPerkCount = i; }
    public void SetDudAddonCount(int i) { Parameters.DudAddonCount = i; }
    public void SetDudItemCount(int i) { Parameters.DudItemCount = i; }

    public void SetRollPerk(bool b) { Parameters.RollPerks = b; }
    public void SetRollAddon(bool b) { Parameters.RollAddons = b; }
    public void SetRollItem(bool b) { Parameters.RollItems = b; }
    public void SetRarityRoll(bool b) { Parameters.RarirtyRoll = b; }
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
