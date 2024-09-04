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

    [SerializeField] GameObject StreakEndedPopup;

    RouletteManager.Result result;

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

    public void RollSurvivor()
    {
        if (Roulette.Parameters.StreakMode)
        {
            if (Roulette.StreakOnGoing)
            {
                if (Roulette.SCharRemaining > 0)
                {
                    result = Roulette.Roll(RouletteManager.MainRollType.Survivor);
                    Roll();
                }
                else
                {
                    StreakEndedPopup.gameObject.SetActive(true);
                }
            }
            else
            {
                result = Roulette.Roll(RouletteManager.MainRollType.Survivor);
                Roll();
            }
            
        }
        else
        {
            result = Roulette.Roll(RouletteManager.MainRollType.Survivor);
            Roll();
        }
    }

    public void RollKiller()
    {
        if (Roulette.Parameters.StreakMode)
        {
            if (Roulette.StreakOnGoing)
            {
                if (Roulette.KCharRemaining > 0)
                {
                    result = Roulette.Roll(RouletteManager.MainRollType.Killer);
                    Roll();
                }
                else
                {
                    StreakEndedPopup.gameObject.SetActive(true);
                }
            }
            else
            {
                result = Roulette.Roll(RouletteManager.MainRollType.Killer);
                Roll();
            }
        }
        else
        {
            result = Roulette.Roll(RouletteManager.MainRollType.Killer);
            Roll();
        }
    }

    public void RollBoth()
    {
        if (Roulette.Parameters.StreakMode)
        {
            if (Roulette.StreakOnGoing)
            {
                if (Roulette.KCharRemaining > 0 || Roulette.SCharRemaining > 0)
                {
                    result = Roulette.Roll(RouletteManager.MainRollType.Both);
                    Roll();
                }
                else
                {
                    StreakEndedPopup.gameObject.SetActive(true);
                }
            }
            else
            {
                result = Roulette.Roll(RouletteManager.MainRollType.Both);
                Roll();
            }    
        }
        else
        {
            result = Roulette.Roll(RouletteManager.MainRollType.Both);
            Roll();
        }
    }

    public void ResetStreak()
    {
        StreakEndedPopup.gameObject.SetActive(false);
        Roulette.ResetStreak();
    }
}
