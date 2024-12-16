using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnlocked : MonoBehaviour
{
    [Serializable]
    public class ClickableEquipableElement
    {
        public Rarity.RarityTypes Rarity;

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

    [Space(5)]

    public List<Items> Items;

    public List<ClickableEquipableItem> ItemsSlot;
    public List<ClickableEquipableElement> Addons;

    List<ClickableEquipableItem> pendingChangeItem = new List<ClickableEquipableItem>();

    List<ClickableEquipableElement> pendingChange = new List<ClickableEquipableElement>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    void OnEnable()
    {
        Setup();
        CheckState();
        UpdateVisual(true);
    }

    void Update()
    {
        UpdateVisual();
    }

    public void Setup()
    {
        if (Items.Count == 0) return;

        foreach (var item in ItemsSlot)
        {
            item.ItemSlot.gameObject.SetActive(false);
        }

        for (int i = 0; i < Items.Count; i++)
        {
            ItemsSlot[i].ItemSlot.Item = Items[i];
            ItemsSlot[i].ItemSlot.gameObject.SetActive(true);
            ItemsSlot[i].ItemSlot.Setup();
        }

        foreach (var item in Addons)
        {
            item.AddonSlot.gameObject.SetActive(false);
        }

        foreach (var item in Items[0].ItemAddons)
        {
            foreach (var addonSlot in Addons)
            {
                if (!addonSlot.AddonSlot.gameObject.activeSelf)
                {
                    if (addonSlot.Rarity == item.RarityType)
                    {
                        addonSlot.AddonSlot.Equipable = item;
                        addonSlot.AddonSlot.gameObject.SetActive(true);
                        addonSlot.AddonSlot.Setup();
                        break;
                    }
                }
            }
        }
    }

    void UpdateVisual(bool forceState = false)
    {
        foreach (var item in pendingChange)
        {
            if (item.State)
            {
                item.Lerp += Time.deltaTime * 5f;
                if (forceState) item.Lerp = 1;
            }
            else
            {
                item.Lerp -= Time.deltaTime * 5f;
                if (forceState) item.Lerp = 0;
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
                if (forceState) item.Lerp = 1;
            }
            else
            {
                item.Lerp -= Time.deltaTime * 5f;
                if (forceState) item.Lerp = 0;
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
        foreach (var item in Addons)
        {
            if (item.AddonSlot == addonSlot)
            {
                item.State = !item.State;

                pendingChange.Add(item);

                DatasManagerV2.Instance.UpdateState(item.AddonSlot.Equipable, item.State);

                break;
            }
        }
    }

    public void ChangeItemState(ItemSlot itemSlot)
    {
        foreach (var item in ItemsSlot)
        {
            if (item.ItemSlot == itemSlot)
            {
                item.State = !item.State;

                pendingChangeItem.Add(item);

                DatasManagerV2.Instance.UpdateState(item.ItemSlot.Item, item.State);

                break;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            foreach (var item in ItemsSlot)
            {
                if(item.ItemSlot != itemSlot)
                {
                    item.State = !item.State;

                    pendingChangeItem.Add(item);

                    DatasManagerV2.Instance.UpdateState(item.ItemSlot.Item, item.State);
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            foreach (var item in Addons)
            {
                ChangeAddonsState(item.AddonSlot);
            }
        }
    }

    public void CheckState()
    {
        foreach (var item in ItemsSlot)
        {
            item.State = DatasManagerV2.Instance.GetState(item.ItemSlot.Item);
            pendingChangeItem.Add(item);
        }

        foreach (var item in Addons)
        {
            item.State = DatasManagerV2.Instance.GetState(item.AddonSlot.Equipable);
            pendingChange.Add(item);
        }
    }

    public void InvertItems()
    {
        foreach (var item in ItemsSlot)
        {
            ChangeItemState(item.ItemSlot);
        }
    }

    public void InvertAddons()
    {
        foreach (var item in Addons)
        {
            ChangeAddonsState(item.AddonSlot);
        }
    }
}
