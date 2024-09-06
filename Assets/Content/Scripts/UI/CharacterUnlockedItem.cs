using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUnlockedItem : MonoBehaviour
{
    [Serializable]
    public class ClickableEquipableElement
    {
        public AddonSlot AddonSlot;

        public bool State = true;

        [HideInInspector] public float Lerp = 1;
    }

    public Characters Character;

    [Space(5)]

    public CharacterSlot CharacterSlot;

    [Space(5)]

    [SerializeField] TMPro.TextMeshProUGUI Name;

    [Space(5)]

    public List<ClickableEquipableElement> Perks;
    
    public List<ClickableEquipableElement> Addons;

    public bool CharacterState = true;

    [SerializeField] GameObject AddonsRoot;

    bool lastCharacterState = true;

    float characterLerp = 1;

    List<ClickableEquipableElement> pendingChange = new List<ClickableEquipableElement>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    void OnEnable()
    {
        Setup();
    }

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

        if(lastCharacterState != CharacterState)
        {
            if (CharacterState)
            {
                characterLerp += Time.deltaTime * 5f;
            }
            else
            {
                characterLerp -= Time.deltaTime * 5f;
            }

            characterLerp = Mathf.Clamp01(characterLerp);


            if (CharacterSlot) CharacterSlot.Icon.color = Color.Lerp(offColor, Color.white, characterLerp);
        }

        if((CharacterState && characterLerp >= 1) || (!CharacterState && characterLerp <= 0))
        {
            lastCharacterState = CharacterState;
        }
    }

    public void ChangePerkState(AddonSlot addonSlot)
    {
        bool state = true;

        foreach (var item in Perks)
        {
            if (item.AddonSlot == addonSlot)
            {
                item.State = state = !item.State;

                pendingChange.Add(item);

                break;
            }
        }

        if (Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledKillerPerks)
            {
                if (item.Perk == addonSlot.Equipable as Perks)
                {
                    item.Enabled = state;
                }
            }
        }
        else
        {
            foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledSurvivorPerks)
            {
                if (item.Perk == addonSlot.Equipable as Perks)
                {
                    item.Enabled = state;
                }
            }
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

        if(Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var killer in RouletteManager.Instance.EnabledDatas.EnabledKillers)
            {
                if (killer.Character == Character)
                {
                    foreach (var addon in killer.EnabledKillerAddons)
                    {
                        if (addon.Addon == addonSlot.Equipable as Addons)
                        {
                            addon.Enabled = state;
                        }
                    }
                }
            }
        }
    }

    public void ChangeCharacterState()
    {
        CharacterState = !CharacterState;

        if(Character.Type == Characters.CharacterType.Killers)
        {
            foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledKillers)
            {
                if (item.Character == Character)
                {
                    item.Enabled = CharacterState;
                }
            }
        }
        else
        {
            foreach (var item in RouletteManager.Instance.EnabledDatas.EnabledSurvivors)
            {
                if (item.Character == Character)
                {
                    item.Enabled = CharacterState;
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            foreach (var perk in Perks)
            {
                ChangePerkState(perk.AddonSlot);
            }
        }
    }

    [ContextMenu("Setup")]
    public void Setup()
    {
        if (!Character) return;

        if (CharacterSlot)
        {
            CharacterSlot.Character = Character;
            CharacterSlot.Setup();
        }

        if (Name)
        {
            Name.text = Character.Name;
        }

        if (Perks.Count == 3)
        {
            for (int i = 0; i < Perks.Count; i++)
            {
                if (i < Character.UniquePerks.Count && Character.UniquePerks[i] != null)
                {
                    Perks[i].AddonSlot.Equipable = Character.UniquePerks[i];

                    Perks[i].AddonSlot.Setup();
                }
            }
        }

        if (Character.Type == Characters.CharacterType.Killers && Addons.Count == 20)
        {
            if (AddonsRoot) AddonsRoot.SetActive(true);

            for (int i = 0; i < Addons.Count; i++)
            {
                if (i < Character.KillerAddons.Count && Character.KillerAddons[i] != null)
                {
                    Addons[i].AddonSlot.Equipable = Character.KillerAddons[i];

                    Addons[i].AddonSlot.Setup();
                }
            }
        }
        else
        {
            if (AddonsRoot) AddonsRoot.SetActive(false);
        }
    }

    public void CheckState()
    {
        foreach (var perk in Perks)
        {
            pendingChange.Add(perk);
        }

        foreach (var addon in Addons)
        {
            pendingChange.Add(addon);
        }
    }

}
