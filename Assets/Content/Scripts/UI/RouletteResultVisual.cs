using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteResultVisual : MonoBehaviour
{
    [SerializeField] bool IsOneShot = false;

    [SerializeField] CharacterSlot Character;

    [SerializeField] ItemSlot Item;

    [SerializeField] List<AddonSlot> PerksSlot;
    [SerializeField] List<AddonSlot> AddonsSlot;

    bool hasSubscribe = false;

    void OnEnable()
    {
        if (RouletteManager.Instance)
        {
            if (IsOneShot)
            {
                ShowResult(RouletteManager.Instance.Results);
                hasSubscribe = true;
            }
            else
            {
                RouletteManager.Instance.OnRollMade += ShowResult;
            }
        }
    }

    private void OnDisable()
    {
        if (RouletteManager.Instance)
        {
            if (hasSubscribe)
            {
                RouletteManager.Instance.OnRollMade -= ShowResult;
                hasSubscribe = false;
            }
        }
    }

    public void ShowResult(RouletteManager.Result result)
    {
        if (Character)
        {
            Character.gameObject.SetActive(true);
            Character.Character = result.Character;
            Character.Setup();
        }
        else
        {
            Character.gameObject.SetActive(false);
        }

        if (Item)
        {
            if (result.Character.Type == Characters.CharacterType.Survivors && result.Item != null && result.Item != RouletteManager.Instance.Duds.ItemDud)
            {
                Item.gameObject.SetActive(true);

                Item.Item = result.Item;
                Item.Setup();
            }
            else
            {
                Item.gameObject.SetActive(false);
            }
        }

        if (PerksSlot.Count == 4)
        {
            for (int i = 0; i < PerksSlot.Count; i++)
            {
                PerksSlot[i].gameObject.SetActive(false);
            }

            foreach (Perks perk in result.Perks)
            {
                if (perk != RouletteManager.Instance.Duds.PerkDud)
                {
                    for (int i = 0; i < PerksSlot.Count; i++)
                    {
                        if (!PerksSlot[i].gameObject.activeSelf)
                        {
                            PerksSlot[i].gameObject.SetActive(true);
                            PerksSlot[i].Equipable = perk;
                            PerksSlot[i].Setup();
                            break;
                        }
                    }
                }
            }
        }

        if (AddonsSlot.Count == 2)
        {
            for (int i = 0; i < AddonsSlot.Count; i++)
            {
                AddonsSlot[i].gameObject.SetActive(false);
            }

            foreach (Addons addon in result.Addons)
            {
                if (addon != RouletteManager.Instance.Duds.AddonDud)
                {
                    for (int i = 0; i < AddonsSlot.Count; i++)
                    {
                        if (!AddonsSlot[i].gameObject.activeSelf)
                        {
                            AddonsSlot[i].gameObject.SetActive(true);
                            AddonsSlot[i].Equipable = addon;
                            AddonsSlot[i].Setup();
                            break;
                        }
                    }
                }
            }
        }

        if (IsOneShot)
        {
            enabled = false;
        }
    }

    public void ResetVisual()
    {
        Character.gameObject.SetActive(false);

        Item.gameObject.SetActive(false);

        foreach (var item in AddonsSlot)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in PerksSlot)
        {
            item.gameObject.SetActive(false);
        }
    }
}
