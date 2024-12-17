using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Serializable]
    public class BaseCounter
    {
        [HideInInspector] public string Name;
        public BaseScriptable Ref;
        public int Count = 0;
        public int Victory = 0;
        public int Lose = 0;
    }

    [Serializable]
    public class ItemCounter
    {
        public List<BaseCounter> ItemsCounted = new List<BaseCounter>();

        public int SurvivorVictory = 0;
        public int KillerVictory = 0;

        public int SurvivorLose = 0;
        public int KillerLose = 0;

        public int KillerCounted
        {
            get
            {
                int i = 0;

                foreach (var item in ItemsCounted)
                {
                    Characters current = item.Ref is Characters ? item.Ref as Characters : null;

                    if (current != null)
                    {
                        if (current.Type == Characters.CharacterType.Killers) i++;
                    }
                }

                return i;
            }
        }
        public int SurvivorCounted
        {
            get
            {
                int i = 0;

                foreach (var item in ItemsCounted)
                {
                    Characters current = item.Ref is Characters ? item.Ref as Characters : null;

                    if (current != null)
                    {
                        if (current.Type == Characters.CharacterType.Survivors) i++;
                    }
                }

                return i;
            }
        }

        public void ReorganizeDescending()
        {
            ItemsCounted = ItemsCounted.OrderByDescending(x => x.Count).ToList();
        }

        public void ReorganizeAcscending()
        {
            ItemsCounted = ItemsCounted.OrderByDescending(x => x.Count).ToList();
            ItemsCounted.Reverse();
        }

        public void AddEntry(BaseScriptable Ref)
        {
            bool found = false;

            foreach (var item in ItemsCounted)
            {
                if (item.Ref == Ref)
                {
                    item.Count += 1;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                BaseCounter newEntry = new BaseCounter();

                if (Ref is Characters)
                {
                    Characters chara = Ref as Characters;
                    newEntry.Name = chara.Name;
                }

                if (Ref is Perks)
                {
                    Perks perk = Ref as Perks;
                    newEntry.Name = perk.Name;
                }

                if (Ref is Addons)
                {
                    Addons addon = Ref as Addons;
                    newEntry.Name = addon.Name;
                }

                if (Ref is Items)
                {
                    Items item = Ref as Items;
                    newEntry.Name = item.Name;
                }

                newEntry.Ref = Ref;
                newEntry.Count += 1;

                ItemsCounted.Add(newEntry);
            }
        }

        public void UpdateVictories(RouletteManager.Result result, bool isVictory)
        {
            if (result.Character == null) return;

            RouletteManager.MainRollType roll = RouletteManager.MainRollType.Killer;
            if (result.Character.Type == Characters.CharacterType.Survivors) roll = RouletteManager.MainRollType.Survivor;

            if (roll == RouletteManager.MainRollType.Killer)
            {
                if (isVictory)
                {
                    KillerVictory++;
                }
                else
                {
                    KillerLose++;
                }
            }
            else
            {
                if (isVictory)
                {
                    SurvivorVictory++;
                }
                else
                {
                    SurvivorLose++;
                }
            }

            UpdateVictory(result.Character, isVictory);

            foreach (var item in result.Perks)
            {
                if (item != null && item != RouletteManager.Instance.Duds.PerkDud)
                {
                    UpdateVictory(item, isVictory);
                }
            }

            if (roll == RouletteManager.MainRollType.Killer)
            {
                foreach (var item in result.Addons)
                {
                    if (item != null && item != RouletteManager.Instance.Duds.AddonDud)
                    {
                        UpdateVictory(item, isVictory);
                    }
                }
            }
            else if(roll == RouletteManager.MainRollType.Survivor)
            {
                if (result.Item != null || RouletteManager.Instance.Duds.ItemDud)
                {
                    UpdateVictory(result.Item, isVictory);

                    foreach (var item in result.Addons)
                    {
                        if (item != null && item != RouletteManager.Instance.Duds.AddonDud)
                        {
                            UpdateVictory(item, isVictory);
                        }
                    }
                }
            }
        }

        void UpdateVictory(BaseScriptable Ref, bool isVictory)
        {
            foreach (var item in ItemsCounted)
            {
                if (item.Ref == Ref)
                {
                    if (isVictory)
                    {
                        item.Victory++;
                        break;
                    }
                    else
                    {
                        item.Lose++;
                        break;
                    }
                }
            }
        }

        public void ResetVictories()
        {
            SurvivorVictory = 0;
            KillerVictory = 0;

            SurvivorLose = 0;
            KillerLose = 0;

            foreach (var item in ItemsCounted)
            {
                item.Victory = 0;
                item.Lose = 0;
            }
        }

        public void Reset()
        {
            ResetVictories();

            ItemsCounted = new List<BaseCounter>();
        }

        public int GetCount(BaseScriptable Ref)
        {
            foreach (var item in ItemsCounted)
            {
                if (item.Ref == Ref)
                {
                    return item.Count;
                }
            }

            return -1;
        }

        public int GetVictories(BaseScriptable Ref)
        {
            foreach (var item in ItemsCounted)
            {
                if (item.Ref == Ref)
                {
                    return item.Victory;
                }
            }

            return -1;
        }

        public int GetLost(BaseScriptable Ref)
        {
            foreach (var item in ItemsCounted)
            {
                if (item.Ref == Ref)
                {
                    return item.Lose;
                }
            }

            return -1;
        }
    }

    [Serializable]
    public class StatsTracked 
    {
        [ReadOnly] public int rollMade;

        [Space(10)]

        [ReadOnly] public int survivors;
        [ReadOnly] public int killers;

        [Space(10)]

        [ReadOnly] public int dudPerks;
        [ReadOnly] public int dudAddons;
        [ReadOnly] public int dudItems;

        [Space(10)]

        [ReadOnly] public int commonCount;
        [ReadOnly] public int uncommonCount;
        [ReadOnly] public int rareCount;
        [ReadOnly] public int veryRareCount;
        [ReadOnly] public int ultraRareCount;

        [Space(10)]

        [ReadOnly] public float survivorPercent;
        [ReadOnly] public float killerPercent;

        [Space(10)]

        [ReadOnly] public float commonPercent;
        [ReadOnly] public float uncommonPercent;
        [ReadOnly] public float rarePercent;
        [ReadOnly] public float veryRarePercent;
        [ReadOnly] public float ultraRarePercent;

        [Space(10)]

        [ReadOnly] public float dudPerksPercent;
        [ReadOnly] public float dudAddonsPercent;
        [ReadOnly] public float dudItemsPercent;
    }

    
    public StatsTracked Stats;
    public ItemCounter Items;

    public Action OnEntryAdded;
    public Action OnDataUpdated;

    [SerializeField] BoolData SaveStatsBoolData;

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


    void OnEnable()
    {
        RouletteManager.Instance.OnRollMade += AddData;
    }
    void OnDisable()
    {
        RouletteManager.Instance.OnRollMade -= AddData;
    }


    void AddData(RouletteManager.Result result)
    {
        if (!DatasManagerV2.Instance.GetSetting(SaveStatsBoolData)) return;

        Stats.rollMade += 1;

        if (result.Character.Type == Characters.CharacterType.Killers)
        {
            Stats.killers += 1;
        }
        else
        {
            Stats.survivors += 1;
        }

        foreach (var item in result.Perks)
        {
            if (item == RouletteManager.Instance.Duds.PerkDud)
            {
                Stats.dudPerks += 1;
            }
        }

        foreach (var item in result.Addons)
        {
            if (item == RouletteManager.Instance.Duds.AddonDud)
            {
                Stats.dudAddons += 1;
            }
            else
            {
                switch (item.RarityType)
                {
                    case Rarity.RarityTypes.Common:
                        Stats.commonCount += 1;
                        break;

                    case Rarity.RarityTypes.Uncommon:
                        Stats.uncommonCount += 1;
                        break;

                    case Rarity.RarityTypes.Rare:
                        Stats.rareCount += 1;
                        break;

                    case Rarity.RarityTypes.VeryRare:
                        Stats.veryRareCount += 1;
                        break;

                    case Rarity.RarityTypes.UltraRare:
                        Stats.ultraRareCount += 1;
                        break;
                }
            }
        }

        if (result.Item != null)
        {
            if (result.Item == RouletteManager.Instance.Duds.ItemDud)
            {
                Stats.dudItems += 1;
            }
            else
            {
                switch (result.Item.Rarity)
                {
                    case Rarity.RarityTypes.Common:
                        Stats.commonCount += 1;
                        break;

                    case Rarity.RarityTypes.Uncommon:
                        Stats.uncommonCount += 1;
                        break;

                    case Rarity.RarityTypes.Rare:
                        Stats.rareCount += 1;
                        break;

                    case Rarity.RarityTypes.VeryRare:
                        Stats.veryRareCount += 1;
                        break;

                    case Rarity.RarityTypes.UltraRare:
                        Stats.ultraRareCount += 1;
                        break;
                }
            }
        }

        AddEntries(result);

        UpdateData();
    }
    void AddEntries(RouletteManager.Result result)
    {
        Items.AddEntry(result.Character);

        foreach (var item in result.Perks)
        {
            if (item != null && item != RouletteManager.Instance.Duds.PerkDud)
            {
                Items.AddEntry(item);
            }
        }

        foreach (var item in result.Addons)
        {
            if (item != null && item != RouletteManager.Instance.Duds.AddonDud)
            {
                Items.AddEntry(item);
            }
        }

        if (result.Item != null)
        {
            Items.AddEntry(result.Item);
        }

        OnEntryAdded?.Invoke();
    }

    void UpdateData()
    {
        Stats.survivorPercent = (float)Stats.survivors / (float)Stats.rollMade * 100f;
        Stats.killerPercent = (float)Stats.killers / (float)Stats.rollMade * 100f;

        Stats.dudPerksPercent = (float)Stats.dudPerks / (float)Stats.rollMade * 100f;
        Stats.dudAddonsPercent = (float)Stats.dudAddons / (float)Stats.rollMade * 100f;
        Stats.dudItemsPercent = (float)Stats.dudItems / (float)Stats.rollMade * 100f;

        Stats.commonPercent = (float)Stats.commonCount / (float)Stats.rollMade * 100f;
        Stats.uncommonPercent = (float)Stats.uncommonCount / (float)Stats.rollMade * 100f;
        Stats.rarePercent = (float)Stats.rareCount / (float)Stats.rollMade * 100f;
        Stats.veryRarePercent = (float)Stats.veryRareCount / (float)Stats.rollMade * 100f;
        Stats.ultraRarePercent = (float)Stats.ultraRareCount / (float)Stats.rollMade * 100f;

        OnDataUpdated?.Invoke();
    }

    public void UpdateVictories(RouletteManager.Result result, bool isVictory)
    {
        Items.UpdateVictories(result, isVictory);
        OnDataUpdated?.Invoke();
    }

    [ContextMenu("Roll 1000 Survivors")]
    void Roll1000Survivors()
    {
        for (int i = 0; i < 1000; i++)
        {
            RouletteManager.Instance.RollSurvivor();
        }
    }
    [ContextMenu("Roll 1000 Killers")]
    void Roll1000Killer()
    {
        for (int i = 0; i < 1000; i++)
        {
            RouletteManager.Instance.RollKiller();
        }
    }
    [ContextMenu("Roll 1000 Both")]
    void Roll1000Both()
    {
        for (int i = 0; i < 1000; i++)
        {
            RouletteManager.Instance.RollBoth();
        }
    }
    [ContextMenu("Reset Stats")]
    public void ResetStats()
    {
        Stats.rollMade = 0;

        Stats.survivors = 0;
        Stats.killers = 0;

        Stats.dudPerks = 0;
        Stats.dudAddons = 0;
        Stats.dudItems = 0;

        Stats.survivorPercent = 0f;
        Stats.killerPercent = 0f;

        Stats.dudPerksPercent = 0f;
        Stats.dudAddonsPercent = 0f;
        Stats.dudItemsPercent = 0f;

        Stats.commonCount = 0;
        Stats.uncommonCount = 0;
        Stats.rareCount = 0;
        Stats.veryRareCount = 0;
        Stats.ultraRareCount = 0;

        Stats.commonPercent = 0f;
        Stats.uncommonPercent = 0f;
        Stats.rarePercent = 0f;
        Stats.veryRarePercent = 0f;
        Stats.ultraRarePercent = 0f;

        Items.Reset();
    }
}
