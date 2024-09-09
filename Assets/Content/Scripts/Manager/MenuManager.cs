using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    class RectEnabled
    {
        public Characters Character;
        public Equipables Equipable;
        public RectTransform Tr = new RectTransform();
        public bool State = true;
    }

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

    List<CharacterSlot> characters = new List<CharacterSlot>();

    List<AddonSlot> equipables = new List<AddonSlot>();

    List<RectEnabled> characterTrs = new List<RectEnabled>();
    List<RectEnabled> equipableTrs = new List<RectEnabled>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    void SetupRemainers(RouletteManager.MainRollType rollType)
    {
        foreach (Transform child in RemainerCharactersScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in RemainerEquipableScroll.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        characters = new List<CharacterSlot>();

        RemainerCharactersScroll.Max = 0;

        List<RouletteManager.EnabledCharacter> enabledCharacters = new List<RouletteManager.EnabledCharacter>();

        switch (rollType)
        {
            case RouletteManager.MainRollType.Killer:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledKillers);
                break;

            case RouletteManager.MainRollType.Survivor:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivors);
                break;

            case RouletteManager.MainRollType.Both:
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledKillers);
                enabledCharacters.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivors);
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

                RectEnabled newRect = new RectEnabled();

                newRect.Character = nchara.Character;

                newRect.Tr = nchara.GetComponent<RectTransform>();

                newRect.State = character.Enabled;

                characterTrs.Add(newRect);

                characters.Add(nchara);
            }
        }

        equipables = new List<AddonSlot>();

        RemainerEquipableScroll.Max = 0;

        List<RouletteManager.EnabledPerk> enabledAddons = new List<RouletteManager.EnabledPerk>();

        switch (rollType)
        {
            case RouletteManager.MainRollType.Killer:
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledKillerPerks);
                break;

            case RouletteManager.MainRollType.Survivor:
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivorPerks);
                break;

            case RouletteManager.MainRollType.Both:
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledKillerPerks);
                enabledAddons.AddRange(RouletteManager.Instance.CurrentList.EnabledSurvivorPerks);

                break;
        }

        foreach (RouletteManager.EnabledPerk perk in enabledAddons)
        {
            if ((RouletteManager.Instance.Results.Perks.Contains(perk.Perk) || perk.Enabled) && perk.Perk != RouletteManager.Instance.Duds.PerkDud)
            {
                GameObject nperkgo = Instantiate(EquipableSlotGo, RemainerEquipableScroll.transform);

                AddonSlot nperk = nperkgo.GetComponent<AddonSlot>();

                nperk.Equipable = perk.Perk;

                nperk.Setup();

                RectEnabled newRect = new RectEnabled();

                newRect.Equipable = nperk.Equipable;

                newRect.Tr = nperk.GetComponent<RectTransform>();

                newRect.State = perk.Enabled;

                equipableTrs.Add(newRect);

                equipables.Add(nperk);
            }
        }

        UpdateRemainers();
    }

    void UpdateRemainers()
    {
        foreach (CharacterSlot character in characters)
        {
            if (RouletteManager.Instance.Parameters.CharacterStreakMode)
            {
                if (character.Character == RouletteManager.Instance.Results.Character)
                {
                    character.Icon.color = offColor;

                    foreach (var item in characterTrs)
                    {
                        if (item.Character == character.Character)
                        {
                            item.State = false;
                        }
                    }
                }
            }     
        }

        List<RectEnabled> tmpTrue = new List<RectEnabled>();
        List<RectEnabled> tmpFalse = new List<RectEnabled>();

        foreach (var item in characterTrs)
        {
            if (item.State)
            {
                tmpTrue.Add(item);
            }
            else
            {
                tmpFalse.Add(item);
            }
        }

        characterTrs = new List<RectEnabled>();
        characterTrs.AddRange(tmpTrue);
        characterTrs.AddRange(tmpFalse);

        SetRect(characterTrs, 10f, RemainerCharactersScroll);

        foreach (AddonSlot equipable in equipables)
        {
            if (RouletteManager.Instance.Parameters.PerkStreakMode)
            {
                foreach (Perks perk in RouletteManager.Instance.Results.Perks)
                {
                    if (equipable.Equipable as Perks == perk)
                    {
                        equipable.Icon.color = offColor;

                        foreach (var item in equipableTrs)
                        {
                            if (item.Equipable == equipable.Equipable)
                            {
                                item.State = false;
                            }
                        }
                    }
                }
            }
        }

        tmpTrue = new List<RectEnabled>();
        tmpFalse = new List<RectEnabled>();

        foreach (var item in equipableTrs)
        {
            if (item.State)
            {
                tmpTrue.Add(item);
            }
            else
            {
                tmpFalse.Add(item);
            }
        }

        equipableTrs = new List<RectEnabled>();
        equipableTrs.AddRange(tmpTrue);
        equipableTrs.AddRange(tmpFalse);

        SetRect(equipableTrs, 10f, RemainerEquipableScroll);

    }

    void SetRect(List<RectEnabled> trs, float divider, CustomScroll scroll)
    {
        int line = 1;
        int column = 0;

        for (int i = 0; i < trs.Count; i++)
        {
            trs[i].Tr.anchorMin = new Vector2(column / divider, 1 - (1 / divider) * line);
            trs[i].Tr.anchorMax = new Vector2((column + 1.0f) / divider, 1 - (1 / divider) * line + (1 / divider));

            column++;

            if (column > divider - 1)
            {
                column = 0;
                line++;

                if (line > divider + 1)
                {
                    scroll.Max += 1 / divider;
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
                StreakScroll.Max += (1 / 3f);
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
                    UpdateRemainers();
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
        characterTrs = new List<RectEnabled>();
        equipableTrs = new List<RectEnabled>();

        characters = new List<CharacterSlot>();
        equipables = new List<AddonSlot>();

        StreakModeVisual.ResetVisual();
    }

    public void CheckStreakMode()
    {
        StreakModeVisual.ResetVisual();
        NormalModeVisual.ResetVisual();

        if (RouletteManager.Instance.Parameters.CharacterStreakMode || RouletteManager.Instance.Parameters.PerkStreakMode)
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
