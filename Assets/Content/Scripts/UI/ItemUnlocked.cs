using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnlocked : MonoBehaviour
{
    [Serializable]
    public class ClickableEquipableElement
    {
        public AddonSlot AddonSlot;

        public bool State = true;

        [HideInInspector] public float Lerp = 1;
    }

    [Serializable]
    public class ClickableEquipableItem
    {
        public ItemSlot ItemSlot;

        public bool State = true;

        [HideInInspector] public float Lerp = 1;
    }

    //[SerializeField] Items Item;

    //[SerializeField] ItemSlot ItemSlot;

    //[SerializeField] TMPro.TextMeshProUGUI Name;

    [Space(5)]

    public List<ClickableEquipableItem> Items;
    public List<ClickableEquipableElement> Addons;

    //public bool ItemState = true;

    //bool lastItemState = true;

    //float itemLerp = 1;

    List<ClickableEquipableItem> pendingChangeItem = new List<ClickableEquipableItem>();

    List<ClickableEquipableElement> pendingChange = new List<ClickableEquipableElement>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    void Update()
    {
        foreach (var item in pendingChange)
        {
            if (item.State)
            {
                item.Lerp += Time.deltaTime * 5f;
            }
            else
            {
                item.Lerp -= Time.deltaTime * 5f;
            }

            item.Lerp = Mathf.Clamp01(item.Lerp);

            item.AddonSlot.Icon.color = Color.Lerp(offColor, Color.white, item.Lerp);
            item.AddonSlot.BG.color = Color.Lerp(offColor, Color.white, item.Lerp);
        }

        for (int i = pendingChange.Count - 1; i >= 0; i--)
        {
            if ((pendingChange[i].State && pendingChange[i].Lerp >= 1) || (!pendingChange[i].State && pendingChange[i].Lerp <= 0)) pendingChange.RemoveAt(i);
        }

        foreach (var item in pendingChangeItem)
        {
            if (item.State)
            {
                item.Lerp += Time.deltaTime * 5f;
            }
            else
            {
                item.Lerp -= Time.deltaTime * 5f;
            }

            item.Lerp = Mathf.Clamp01(item.Lerp);

            item.ItemSlot.Icon.color = Color.Lerp(offColor, Color.white, item.Lerp);
            item.ItemSlot.BG.color = Color.Lerp(offColor, Color.white, item.Lerp);
        }

        for (int i = pendingChange.Count - 1; i >= 0; i--)
        {
            if ((pendingChange[i].State && pendingChange[i].Lerp >= 1) || (!pendingChange[i].State && pendingChange[i].Lerp <= 0)) pendingChange.RemoveAt(i);
        }
    }

    public void ChangeAddonsState(AddonSlot addonSlot)
    {
        bool state = true;

        foreach (var item in Addons)
        {
            if (item.AddonSlot == addonSlot)
            {
                item.State = state = !item.State;

                pendingChange.Add(item);

                break;
            }
        }

        foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledItems)
        {
            foreach (var addon in item.EnabledItemAddons)
            {
                if(addon.Addon == addonSlot.Equipable as Addons) 
                {
                    addon.Enabled = state;
                }
            }
        }
    }

    public void ChangeItemState(ItemSlot itemSlot)
    {
        bool state = true;

        foreach (var item in Items)
        {
            if (item.ItemSlot == itemSlot)
            {
                item.State = state = !item.State;

                pendingChangeItem.Add(item);

                break;
            }
        }

        foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledItems)
        {
            if(item.Item == itemSlot.Item)
            {
                item.Enabled = state;
            }
        }
    }
}
