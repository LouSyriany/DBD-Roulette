using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : Slot
{
    public Items Item;

    [SerializeField] Sprite[] BgRarity;

    void OnValidate()
    {
        Setup();
    }

    void OnEnable()
    {
        Setup();
    }

    public void Setup()
    {
        if (Icon && BG)
        {
            if (Item == null)
            {
                Icon.gameObject.SetActive(false);
            }
            else
            {
                Icon.gameObject.SetActive(true);
                Icon.sprite = Item.Icon;

                if (BgRarity.Length == 6)
                {
                    switch (Item.Rarity)
                    {
                        case Rarity.RarityTypes.Common:
                            BG.sprite = BgRarity[0];
                            break;

                        case Rarity.RarityTypes.Uncommon:
                            BG.sprite = BgRarity[1];
                            break;

                        case Rarity.RarityTypes.Rare:
                            BG.sprite = BgRarity[2];
                            break;

                        case Rarity.RarityTypes.VeryRare:
                            BG.sprite = BgRarity[3];
                            break;

                        case Rarity.RarityTypes.UltraRare:
                            BG.sprite = BgRarity[4];
                            break;
                    }
                }
            }

        }
    }
}
