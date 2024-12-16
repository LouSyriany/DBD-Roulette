using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [Serializable]
    public class BaseCounter
    {
        [HideInInspector] public string Name;
        public BaseScriptable Ref;
        public int Count = 0;
    }

    [Serializable]
    public class ItemCounter
    {
        public List<BaseCounter> ItemsCounted = new List<BaseCounter>();

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
            //ItemsCounted.Sort();
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
                newEntry.Name = Ref.name;
                newEntry.Ref = Ref;
                newEntry.Count += 1;

                ItemsCounted.Add(newEntry);
            }
        }

        public void Reset()
        {
            ItemsCounted = new List<BaseCounter>();
        }
    }

    [SerializeField, ReadOnly] int rollMade;

    [Space(10)]

    [SerializeField, ReadOnly] int survivors;
    [SerializeField, ReadOnly] int killers;

    [Space(10)]

    [SerializeField, ReadOnly] int dudPerks;
    [SerializeField, ReadOnly] int dudAddons;
    [SerializeField, ReadOnly] int dudItems;

    [Space(10)]

    [SerializeField, ReadOnly] int commonCount;
    [SerializeField, ReadOnly] int uncommonCount;
    [SerializeField, ReadOnly] int rareCount;
    [SerializeField, ReadOnly] int veryRareCount;
    [SerializeField, ReadOnly] int ultraRareCount;

    [Space(10)]

    [SerializeField, ReadOnly] float survivorPercent;
    [SerializeField, ReadOnly] float killerPercent;

    [Space(10)]

    [SerializeField, ReadOnly] float commonPercent;
    [SerializeField, ReadOnly] float uncommonPercent;
    [SerializeField, ReadOnly] float rarePercent;
    [SerializeField, ReadOnly] float veryRarePercent;
    [SerializeField, ReadOnly] float ultraRarePercent;

    [Space(10)]

    [SerializeField, ReadOnly] float dudPerksPercent;
    [SerializeField, ReadOnly] float dudAddonsPercent;
    [SerializeField, ReadOnly] float dudItemsPercent;

    public ItemCounter Items;

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
        rollMade += 1;

        if (result.Character.Type == Characters.CharacterType.Killers)
        {
            killers += 1;
        }
        else
        {
            survivors += 1;
        }

        foreach (var item in result.Perks)
        {
            if (item == RouletteManager.Instance.Duds.PerkDud)
            {
                dudPerks += 1;
            }
        }

        foreach (var item in result.Addons)
        {
            if (item == RouletteManager.Instance.Duds.AddonDud)
            {
                dudAddons += 1;
            }
            else
            {
                switch (item.RarityType)
                {
                    case Rarity.RarityTypes.Common:
                        commonCount += 1;
                        break;

                    case Rarity.RarityTypes.Uncommon:
                        uncommonCount += 1;
                        break;

                    case Rarity.RarityTypes.Rare:
                        rareCount += 1;
                        break;

                    case Rarity.RarityTypes.VeryRare:
                        veryRareCount += 1;
                        break;

                    case Rarity.RarityTypes.UltraRare:
                        ultraRareCount += 1;
                        break;
                }
            }
        }

        if (result.Item != null)
        {
            if (result.Item == RouletteManager.Instance.Duds.ItemDud)
            {
                dudItems += 1;
            }
            else
            {
                switch (result.Item.Rarity)
                {
                    case Rarity.RarityTypes.Common:
                        commonCount += 1;
                        break;

                    case Rarity.RarityTypes.Uncommon:
                        uncommonCount += 1;
                        break;

                    case Rarity.RarityTypes.Rare:
                        rareCount += 1;
                        break;

                    case Rarity.RarityTypes.VeryRare:
                        veryRareCount += 1;
                        break;

                    case Rarity.RarityTypes.UltraRare:
                        ultraRareCount += 1;
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
    }

    void UpdateData()
    {
        survivorPercent = (float)survivors / (float)rollMade * 100f;
        killerPercent = (float)killers / (float)rollMade * 100f;

        dudPerksPercent = (float)dudPerks / (float)rollMade * 100f;
        dudAddonsPercent = (float)dudAddons / (float)rollMade * 100f;
        dudItemsPercent = (float)dudItems / (float)rollMade * 100f;

        commonPercent = (float)commonCount / (float)rollMade * 100f;
        uncommonPercent = (float)uncommonCount / (float)rollMade * 100f;
        rarePercent = (float)rareCount / (float)rollMade * 100f;
        veryRarePercent = (float)veryRareCount / (float)rollMade * 100f;
        ultraRarePercent = (float)ultraRareCount / (float)rollMade * 100f;
    }

    [ContextMenu("Rearrange List")]
    void RearrangeList()
    {
        Items.ReorganizeDescending();
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
    void ResetStats()
    {
        rollMade = 0;

        survivors = 0;
        killers = 0;

        dudPerks = 0;
        dudAddons = 0;
        dudItems = 0;

        survivorPercent = 0f;
        killerPercent = 0f;

        dudPerksPercent = 0f;
        dudAddonsPercent = 0f;
        dudItemsPercent = 0f;

        commonCount = 0;
        uncommonCount = 0;
        rareCount = 0;
        veryRareCount = 0;
        ultraRareCount = 0;

        commonPercent = 0f;
        uncommonPercent = 0f;
        rarePercent = 0f;
        veryRarePercent = 0f;
        ultraRarePercent = 0f;
    }
}
