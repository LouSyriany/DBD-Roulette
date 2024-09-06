using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] RectTransform NormalMode;
    [SerializeField] RouletteResultVisual NormalModeVisual;

    [SerializeField] RectTransform StreakMode;
    [SerializeField] RouletteResultVisual StreakModeVisual;

    [Space(10)]

    [SerializeField] GameObject StreakEndedPopup;

    [Space(10)]

    [SerializeField] GameObject CharacterSlotGo;

    [SerializeField] GameObject EquipableSlotGo;
    
    [Space(10)]

    [SerializeField] GameObject StreakEntry;

    [SerializeField] CustomScroll StreakScroll;

    [SerializeField] CustomScroll RemainerCharactersScroll;

    [SerializeField] CustomScroll RemainerEquipableScroll;

    List<RectTransform> streakEntries = new List<RectTransform>();

    List<CharacterSlot> characters;

    List<AddonSlot> equipables;

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    void SetupRemainers(RouletteManager.MainRollType rollType)
    {
        characters = new List<CharacterSlot>();

        equipables = new List<AddonSlot>();

        RemainerCharactersScroll.Max = 0;
        RemainerEquipableScroll.Max = 0;

        foreach (Transform child in RemainerCharactersScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in RemainerEquipableScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<RectTransform> characterTrs = new List<RectTransform>();

        List<RectTransform> equipableTrs = new List<RectTransform>();

        List<RouletteManager.EnabledCharacter> enabledCharacters = new List<RouletteManager.EnabledCharacter>();

        List<RouletteManager.EnabledPerk> enabledAddons = new List<RouletteManager.EnabledPerk>();

        switch (rollType)
        {
            case RouletteManager.MainRollType.Killer:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledKillers);
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledKillerPerks);
                break;

            case RouletteManager.MainRollType.Survivor:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivors);
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivorPerks);
                break;

            case RouletteManager.MainRollType.Both:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledKillers);
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivors);

                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledKillerPerks);
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivorPerks);

                break;
        }

        foreach (RouletteManager.EnabledCharacter character in enabledCharacters)
        {
            if (character.Character == RouletteManager.Instance.Results.Character || character.Enabled)
            {
                GameObject nCharago = Instantiate(CharacterSlotGo, RemainerCharactersScroll.transform);

                CharacterSlot nchara = nCharago.GetComponent<CharacterSlot>();

                nchara.Character = character.Character;

                nchara.Setup();

                characterTrs.Add(nCharago.GetComponent<RectTransform>());

                characters.Add(nchara);
            }
        }

        foreach (RouletteManager.EnabledPerk perk in enabledAddons)
        {
            if ((RouletteManager.Instance.Results.Perks.Contains(perk.Perk) || perk.Enabled) && perk.Perk != RouletteManager.Instance.Duds.PerkDud)
            {
                GameObject nperkgo = Instantiate(EquipableSlotGo, RemainerEquipableScroll.transform);

                AddonSlot nperk = nperkgo.GetComponent<AddonSlot>();

                nperk.Equipable = perk.Perk;

                nperk.Setup();

                equipableTrs.Add(nperk.GetComponent<RectTransform>());

                equipables.Add(nperk);
            }
        }

        int line = 1;
        int column = 0;

        float divider = 10f;

        for (int i = 0; i < characterTrs.Count; i++)
        {
            characterTrs[i].anchorMin = new Vector2(column / divider, 1 - (1 / divider) * line);
            characterTrs[i].anchorMax = new Vector2((column + 1.0f) / divider, 1 - (1 / divider) * line + .1f);

            column++;

            if (column > divider - 1)
            {
                column = 0;
                line++;

                if (line > divider + 1)
                {
                    RemainerEquipableScroll.Max += 1 / divider;
                }
            }
        }

        line = 1;
        column = 0;
        divider = 10f;

        for (int i = 0; i < equipableTrs.Count; i++)
        {
            equipableTrs[i].anchorMin = new Vector2(column / divider, 1 - (1 / divider) * line);
            equipableTrs[i].anchorMax = new Vector2((column + 1.0f) / divider, 1 - (1 / divider) * line + (1 / divider));

            column++;

            if (column > divider - 1)
            {
                column = 0;
                line++;

                if (line > divider + 1)
                {
                    RemainerEquipableScroll.Max += 1 / divider;
                }
            }
        }

        UpdateRemainers(rollType);
    }

    void UpdateRemainers(RouletteManager.MainRollType rollType)
    {
        foreach (CharacterSlot character in characters)
        {
            if (character.Character == RouletteManager.Instance.Results.Character)
            {
                character.Icon.color = offColor;
            }
        }

        foreach (AddonSlot equipable in equipables)
        {
            foreach (Perks perk in RouletteManager.Instance.Results.Perks)
            {
                if(equipable.Equipable as Perks == perk)
                {
                    equipable.Icon.color = offColor;
                    break;
                }
            }
        }
    }

    void AddStreakEntry()
    {
        GameObject nEntry = Instantiate(StreakEntry, StreakScroll.transform);

        RectTransform tr = nEntry.GetComponent<RectTransform>();

        streakEntries.Add(tr);

        nEntry.name = "StreakEntry-" + streakEntries.Count;

        for (int i = 0; i < streakEntries.Count; i++)
        {
            streakEntries[streakEntries.Count - 1 - i].anchorMin = new Vector2(0, 1 - (1 / 3f) * (i + 1));
            streakEntries[streakEntries.Count - 1 - i].anchorMax = new Vector2(1, 1 - (1 / 3f) * (i + 1) + (1 / 3f));
        }

        if (StreakScroll)
        {
            if (streakEntries.Count > 3)
            {
                StreakScroll.Max += (1/3f);
            }
        }
    }

    void Roll(RouletteManager.MainRollType rollType)
    {
        if (!RouletteManager.Instance) return;

        if (RouletteManager.Instance.Parameters.CharacterStreakMode || RouletteManager.Instance.Parameters.PerkStreakMode)
        {
            if (RouletteManager.Instance.StreakOnGoing)
            {
                if (!RouletteManager.Instance.StreakStopped)
                {
                    RouletteManager.Instance.Roll(rollType);

                    AddStreakEntry();
                    UpdateRemainers(rollType);
                }
                else
                {
                    StreakEndedPopup.gameObject.SetActive(true);
                }
            }
            else
            {
                RouletteManager.Instance.Roll(rollType);
                AddStreakEntry();
                SetupRemainers(rollType);
            }
        }
        else
        {
            RouletteManager.Instance.Roll(rollType);
        }
    }

    public void RollSurvivor()
    {
        Roll(RouletteManager.MainRollType.Survivor);
    }

    public void RollKiller()
    {
        Roll(RouletteManager.MainRollType.Killer);
    }

    public void RollBoth()
    {
        Roll(RouletteManager.MainRollType.Both);
    }

    public void ResetStreak()
    {
        if (!RouletteManager.Instance) return;

        RouletteManager.Instance.ResetStreak();

        foreach (Transform child in StreakScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in RemainerCharactersScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        foreach (Transform child in RemainerEquipableScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        streakEntries = new List<RectTransform>();

        StreakModeVisual.ResetVisual();
    }

    public void CheckStreakMode()
    {
        StreakModeVisual.ResetVisual();
        NormalModeVisual.ResetVisual();

        if (RouletteManager.Instance.Parameters.CharacterStreakMode  || RouletteManager.Instance.Parameters.PerkStreakMode)
        {
            NormalMode.gameObject.SetActive(false);
            StreakMode.gameObject.SetActive(true);
        }
        else
        {
            ResetStreak();
            NormalMode.gameObject.SetActive(true);
            StreakMode.gameObject.SetActive(false);
        }
    }
}