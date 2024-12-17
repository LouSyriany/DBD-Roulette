using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagerV2 : MonoBehaviour
{
    class RectEnabled
    {
        public Characters Character;
        public Equipables Equipable;
        public RectTransform Tr = new RectTransform();
        public bool State = true;
    }

    [SerializeField] GameObject KillerRef;
    [SerializeField] GameObject SurvivorRef;
    [SerializeField] GameObject ItemRef;

    [Space(10)]

    [SerializeField] RectTransform KillerOptions;
    [SerializeField] RectTransform SurvivorOptions;
    [SerializeField] RectTransform ItemOptions;

    [Space(10)]

    [SerializeField] BoolData PerkStreak;
    [SerializeField] BoolData CharacterStreak;

    [SerializeField] GameObject NormalMode;
    [SerializeField] GameObject StreakMode;

    //-----

    [Space(10)]

    [SerializeField] GameObject StreakEndedPopup;

    [Space(10)]

    [SerializeField] GameObject CharacterSlotGo;

    [SerializeField] GameObject EquipableSlotGo;

    [Space(10)]

    [SerializeField] RouletteResultVisual NormalModeVisual;
    [SerializeField] RouletteResultVisual StreakModeVisual;

    [SerializeField] GameObject StreakEntry;

    [SerializeField] CustomScroll StreakScroll;
    [SerializeField] CustomScroll RemainerCharactersScroll;
    [SerializeField] CustomScroll RemainerEquipableScroll;

    [Space(10)]

    [SerializeField] Button SurvivorStreakButton;
    [SerializeField] Button BothStreakButton;
    [SerializeField] Button KillerStreakButton;

    List<RectTransform> streakEntries = new List<RectTransform>();

    List<CharacterSlot> characters = new List<CharacterSlot>();

    List<AddonSlot> equipables = new List<AddonSlot>();

    List<RectEnabled> characterTrs = new List<RectEnabled>();
    List<RectEnabled> equipableTrs = new List<RectEnabled>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    bool prevValue;

    RouletteManager.Result prevResult;

    void OnEnable()
    {
        RouletteManager.Instance.OnStreakReset += ResetStreak;
        RouletteManager.Instance.OnRollMade += RollMade;
        RouletteManager.Instance.OnRollMade += UpdateRemainers;
        RouletteManager.Instance.OnStreakSetuped += SetupRemainers;
        RouletteManager.Instance.OnStreakStopped += StopStreak;

        DatasManagerV2.Instance.OnStreakDataLoaded += ResetStreak;

        Setup();
    }

    void OnDisable()
    {
        RouletteManager.Instance.OnStreakReset -= ResetStreak;
        RouletteManager.Instance.OnRollMade -= RollMade;
        RouletteManager.Instance.OnRollMade -= UpdateRemainers;
        RouletteManager.Instance.OnStreakSetuped -= SetupRemainers;
        RouletteManager.Instance.OnStreakStopped -= StopStreak;

        DatasManagerV2.Instance.OnStreakDataLoaded -= ResetStreak;
    }

    void FixedUpdate()
    {
        bool streakMode = DatasManagerV2.Instance.GetSetting(PerkStreak) || DatasManagerV2.Instance.GetSetting(CharacterStreak);
        
        NormalMode.SetActive(!streakMode);
        StreakMode.SetActive(streakMode);
    }

    void StopStreak()
    {
        StreakEndedPopup.gameObject.SetActive(true);
    }

    void RollMade(RouletteManager.Result result)
    {
        if (RouletteManager.Instance.StreakOnGoing && result != prevResult)
        {
            AddStreakEntry();
            prevResult = result;
        }

        switch (result.Roll)
        {
            case RouletteManager.MainRollType.Survivor:
                SurvivorStreakButton.interactable = true;
                BothStreakButton.interactable = false;
                KillerStreakButton.interactable = false;
                break;

            case RouletteManager.MainRollType.Both:
                SurvivorStreakButton.interactable = false;
                BothStreakButton.interactable = true;
                KillerStreakButton.interactable = false;
                break;

            case RouletteManager.MainRollType.Killer:
                SurvivorStreakButton.interactable = false;
                BothStreakButton.interactable = false;
                KillerStreakButton.interactable = true;
                break;
        }
    }

    void Setup()
    {
        List<RectTransform> rcts = new List<RectTransform>();

        if (KillerOptions && KillerRef)
        {
            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Killers.Count; i++)
            {
                var current = DatasManagerV2.Instance.DataBase.Killers[i];

                GameObject newKiller = Instantiate(KillerRef, KillerOptions);

                rcts.Add(newKiller.GetComponent<RectTransform>());

                newKiller.GetComponent<CharacterUnlockedItem>().Character = current.Ref as Characters;
                newKiller.name = current.Ref.name;
                newKiller.gameObject.SetActive(true);
            }
            SetRect(rcts, 3, new Vector2(1, 1));
        }

        if (SurvivorOptions && SurvivorRef)
        {
            rcts = new List<RectTransform>();

            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Survivors.Count; i++)
            {
                var current = DatasManagerV2.Instance.DataBase.Survivors[i];

                GameObject newSurvivor = Instantiate(SurvivorRef, SurvivorOptions);

                rcts.Add(newSurvivor.GetComponent<RectTransform>());

                newSurvivor.GetComponent<CharacterUnlockedItem>().Character = current.Ref as Characters;
                newSurvivor.name = current.Ref.name;
                newSurvivor.gameObject.SetActive(true);
            }
            SetRect(rcts, 3f, new Vector2(1, .5f));
        }

        if (ItemOptions && ItemRef)
        {
            rcts = new List<RectTransform>();

            List<ItemUnlocked> alreadyCreated = new List<ItemUnlocked>();

            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Items.Count; i++)
            {
                Items cuItem = DatasManagerV2.Instance.DataBase.Items[i].Ref as Items;

                bool found = false;

                foreach (var item in alreadyCreated)
                {
                    if (item.Items[0].Type == cuItem.Type)
                    {
                        item.Items.Add(cuItem);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    GameObject newItems = Instantiate(ItemRef, ItemOptions);

                    rcts.Add(newItems.GetComponent<RectTransform>());

                    ItemUnlocked iu = newItems.GetComponent<ItemUnlocked>();
                    iu.Items.Add(cuItem);
                    newItems.name = cuItem.Type.ToString();
                    alreadyCreated.Add(iu);
                }
            }

            foreach (var item in alreadyCreated)
            {
                item.gameObject.SetActive(true);
            }

            SetRect(rcts, 2f, new Vector2(1, .5f));
        }
    }

    void SetRect(List<RectTransform> rcts, float divider, Vector2 ratio)
    {
        int line = 1;
        int column = 0;

        for (int i = 0; i < rcts.Count; i++)
        {
            rcts[i].anchorMin = new Vector2(column / divider, 1 - (ratio.y / divider) * line);
            rcts[i].anchorMax = new Vector2((column + 1.0f) / divider, 1 - (ratio.y / divider) * line + (ratio.y / divider));

            column++;

            if (column > divider - 1)
            {
                column = 0;
                line++;
            }
        }
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

        List<DatasManagerV2.DataState> enabledCharacters = new List<DatasManagerV2.DataState>();

        switch (rollType)
        {
            case RouletteManager.MainRollType.Killer:
                enabledCharacters.AddRange(DatasManagerV2.Instance.DataBase.Killers);
                break;

            case RouletteManager.MainRollType.Survivor:
                enabledCharacters.AddRange(DatasManagerV2.Instance.DataBase.Survivors);
                break;

            case RouletteManager.MainRollType.Both:
                enabledCharacters.AddRange(DatasManagerV2.Instance.DataBase.Killers);
                enabledCharacters.AddRange(DatasManagerV2.Instance.DataBase.Survivors);
                break;
        }

        foreach (DatasManagerV2.DataState character in enabledCharacters)
        {
            if (character.Ref as Characters == RouletteManager.Instance.Results.Character || character.State)
            {
                GameObject nCharago = Instantiate(CharacterSlotGo, RemainerCharactersScroll.transform);

                CharacterSlot nchara = nCharago.GetComponent<CharacterSlot>();

                nchara.Character = character.Ref as Characters;

                nchara.Setup();

                RectEnabled newRect = new RectEnabled();

                newRect.Character = nchara.Character;

                newRect.Tr = nchara.GetComponent<RectTransform>();

                newRect.State = character.State;

                characterTrs.Add(newRect);

                characters.Add(nchara);
            }
        }

        equipables = new List<AddonSlot>();

        RemainerEquipableScroll.Max = 0;

        List<DatasManagerV2.DataState> enabledAddons = new List<DatasManagerV2.DataState>();

        switch (rollType)
        {
            case RouletteManager.MainRollType.Killer:
                enabledAddons.AddRange(DatasManagerV2.Instance.DataBase.KillerPerks);
                break;

            case RouletteManager.MainRollType.Survivor:
                enabledAddons.AddRange(DatasManagerV2.Instance.DataBase.SurvivorPerks);
                break;

            case RouletteManager.MainRollType.Both:
                enabledAddons.AddRange(DatasManagerV2.Instance.DataBase.KillerPerks);
                enabledAddons.AddRange(DatasManagerV2.Instance.DataBase.SurvivorPerks);

                break;
        }

        foreach (DatasManagerV2.DataState perk in enabledAddons)
        {
            if ((RouletteManager.Instance.Results.Perks.Contains(perk.Ref as Perks) || perk.State) && perk.Ref != RouletteManager.Instance.Duds.PerkDud)
            {
                GameObject nperkgo = Instantiate(EquipableSlotGo, RemainerEquipableScroll.transform);

                AddonSlot nperk = nperkgo.GetComponent<AddonSlot>();

                nperk.Equipable = perk.Ref as Perks;

                nperk.Setup();

                RectEnabled newRect = new RectEnabled();

                newRect.Equipable = nperk.Equipable;

                newRect.Tr = nperk.GetComponent<RectTransform>();

                newRect.State = perk.State;

                equipableTrs.Add(newRect);

                equipables.Add(nperk);
            }
        }

        //UpdateRemainers();
    }


    public void UpdateRemainers(RouletteManager.Result result)
    {
        if (!DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.CharacterStreakMode)
            || !DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.PerkStreakMode))
        { return; }

        foreach (CharacterSlot character in characters)
        {
            if (DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.CharacterStreakMode))
            {
                if (character.Character == result.Character)
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
            if (DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.PerkStreakMode))
            {
                foreach (Perks perk in result.Perks)
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
                StreakScroll.Max = (streakEntries.Count - 3) * (1 / 3f);
            }
        }

        if (!RouletteManager.Instance.StreakOnGoing)
        {
            if (streakEntries.Count >= 15)
            {
                RectTransform tmp = streakEntries[0];
                streakEntries.RemoveAt(0);
                Destroy(tmp.gameObject);
            }
        }
    }

    void ResetStreak()
    {
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

        if (DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.CharacterStreakMode)
            || DatasManagerV2.Instance.GetSetting(RouletteManager.Instance.Parameters.PerkStreakMode))
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
