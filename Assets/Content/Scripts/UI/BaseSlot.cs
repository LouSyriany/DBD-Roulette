using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSlot : Slot
{
    public BaseScriptable Scriptable;

    [SerializeField] Sprite[] Bg;

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
        if (!Icon && !BG && !Scriptable && Bg.Length != 7) return;

        if (Scriptable is Characters)
        {
            Characters chara = Scriptable as Characters;

            Icon.gameObject.SetActive(true);
            Icon.sprite = chara.Icon;

            BG.sprite = Bg[6];
            
            if (chara.Type == Characters.CharacterType.Killers)
            {
                BG.color = KillerColor;
            }
            else
            {
                BG.color = SurvivorColor;
            }
        }

        if(Scriptable is Addons)
        {
            Addons addon = Scriptable as Addons;

            Icon.gameObject.SetActive(true);
            Icon.sprite = addon.Icon;

            BG.color = Color.white;

            switch (addon.RarityType)
            {
                case Rarity.RarityTypes.Common:
                    BG.sprite = Bg[0];
                    break;

                case Rarity.RarityTypes.Uncommon:
                    BG.sprite = Bg[1];
                    break;

                case Rarity.RarityTypes.Rare:
                    BG.sprite = Bg[2];
                    break;

                case Rarity.RarityTypes.VeryRare:
                    BG.sprite = Bg[3];
                    break;

                case Rarity.RarityTypes.UltraRare:
                    BG.sprite = Bg[4];
                    break;
            }
        }

        if (Scriptable is Perks)
        {
            Perks perk = Scriptable as Perks;

            Icon.gameObject.SetActive(true);
            Icon.sprite = perk.Icon;

            BG.color = Color.white;

            BG.sprite = Bg[5];
        }

        if (Scriptable is Items)
        {
            Items item = Scriptable as Items;

            Icon.gameObject.SetActive(true);
            Icon.sprite = item.Icon;

            BG.color = Color.white;

            switch (item.Rarity)
            {
                case Rarity.RarityTypes.Common:
                    BG.sprite = Bg[0];
                    break;

                case Rarity.RarityTypes.Uncommon:
                    BG.sprite = Bg[1];
                    break;

                case Rarity.RarityTypes.Rare:
                    BG.sprite = Bg[2];
                    break;

                case Rarity.RarityTypes.VeryRare:
                    BG.sprite = Bg[3];
                    break;

                case Rarity.RarityTypes.UltraRare:
                    BG.sprite = Bg[4];
                    break;
            }
        }
    }
}
