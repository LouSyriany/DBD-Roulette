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
        [HideInInspector] public string Name;

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

    public List<ClickableEquipableElement> Perks = new List<ClickableEquipableElement>();
    
    public List<ClickableEquipableElement> Addons = new List<ClickableEquipableElement>();

    public bool CharacterState = true;

    [SerializeField] GameObject AddonsRoot;

    bool lastCharacterState = true;

    float characterLerp = 1;

    List<ClickableEquipableElement> pendingChange = new List<ClickableEquipableElement>();

    Color offColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    private void OnValidate()
    {
        foreach (var addon in Addons)
        {
            if (addon.AddonSlot != null) addon.Name = addon.AddonSlot.name;
        }
    }

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

        if (lastCharacterState != CharacterState)
        {
            if (CharacterState)
            {
                characterLerp += Time.deltaTime * 5f;
                if (forceState) characterLerp = 1;
            }
            else
            {
                characterLerp -= Time.deltaTime * 5f;
                if (forceState) characterLerp = 0;
            }

            characterLerp = Mathf.Clamp01(characterLerp);

            if (CharacterSlot) CharacterSlot.Icon.color = Color.Lerp(offColor, Color.white, characterLerp);
        }

        if ((CharacterState && characterLerp >= 1) || (!CharacterState && characterLerp <= 0))
        {
            lastCharacterState = CharacterState;
        }
    }

    public void ChangePerkState(AddonSlot addonSlot)
    {
        foreach (var item in Perks)
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

    public void ChangeCharacterState()
    {
        CharacterState = !CharacterState;

        DatasManagerV2.Instance.UpdateState(Character, CharacterState);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            foreach (var perk in Perks)
            {
                ChangePerkState(perk.AddonSlot);
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            foreach (var addon in Addons)
            {
                ChangeAddonsState(addon.AddonSlot);
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
        CharacterState = DatasManagerV2.Instance.GetState(Character);

        foreach (var item in Perks)
        {
            item.State = DatasManagerV2.Instance.GetState(item.AddonSlot.Equipable);
            pendingChange.Add(item);
        }

        foreach (var item in Addons)
        {
            item.State = DatasManagerV2.Instance.GetState(item.AddonSlot.Equipable);
            pendingChange.Add(item);
        }
    }


    public void InvertCharacter()
    {
        ChangeCharacterState();
    }

    public void InvertPerks()
    {
        foreach (var item in Perks)
        {
            ChangePerkState(item.AddonSlot);
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
