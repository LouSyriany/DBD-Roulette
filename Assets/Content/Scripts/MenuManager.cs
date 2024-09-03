using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] RouletteManager Roulette;

    [Space(10)]

    [SerializeField] GameObject CharacterSlot;
    [SerializeField] GameObject EquipableSlot;

    [Space(10)]

    [SerializeField] CharacterSlot Character;

    [SerializeField] ItemSlot Item;

    [SerializeField] RectTransform TrPerkList;
    [SerializeField] RectTransform TrAddonList;

    RouletteManager.Result result;

    public void RollSurvivor()
    {
        result = Roulette.Roll(RouletteManager.MainRollType.Survivor);
        Roll();
    }

    public void RollKiller()
    {
        result = Roulette.Roll(RouletteManager.MainRollType.Killer);
        Roll();
    }

    public void RollBoth()
    {
        result = Roulette.Roll(RouletteManager.MainRollType.Both);
        Roll();
    }

    void Roll()
    {
        Character.Character = result.Character;
        Character.Setup();

        if (result.Character.Type == Characters.CharacterType.Survivors && result.Item != null && result.Item != Roulette.Duds.ItemDud)
        {
            Item.gameObject.SetActive(true);

            Item.Item = result.Item;
            Item.Setup();
        }
        else
        {
            Item.gameObject.SetActive(false);
        }

        foreach (Transform child in TrPerkList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int index = 0;

        foreach (var perk in result.Perks)
        {
            if (perk != Roulette.Duds.PerkDud)
            {
                GameObject currengGo = Instantiate(EquipableSlot, TrPerkList);

                RectTransform tr = currengGo.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(index, 0);
                tr.anchorMax = new Vector2(index + 1, 1);

                AddonSlot currentSlot = currengGo.GetComponent<AddonSlot>();
                currentSlot.Equipable = perk;
                currentSlot.Setup();

                index++;
            }
        }


        foreach (Transform child in TrAddonList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        index = 0;

        foreach (var addon in result.Addons)
        {
            if (addon != Roulette.Duds.AddonDud && addon != null)
            {
                GameObject currengGo = Instantiate(EquipableSlot, TrAddonList);

                RectTransform tr = currengGo.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(index, 0);
                tr.anchorMax = new Vector2(index + 1, 1);

                AddonSlot currentSlot = currengGo.GetComponent<AddonSlot>();
                currentSlot.Equipable = addon;
                currentSlot.Setup();

                index++;
            }
        }
    }
}
