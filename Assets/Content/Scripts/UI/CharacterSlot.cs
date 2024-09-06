using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : Slot
{
    public Characters Character;

    void OnEnable()
    {
        Setup();
    }

    [ContextMenu("Setup")]
    public void Setup()
    {
        if (Icon && BG)
        {
            if (Character == null)
            {
                Icon.gameObject.SetActive(false);
            }
            else
            {
                Icon.gameObject.SetActive(true);
                Icon.sprite = Character.Icon;

                if (Character.Type == Characters.CharacterType.Killers)
                {
                    BG.color = KillerColor;
                }
                else
                {
                    BG.color = SurvivorColor;
                }
            }
            
        }
    }
}
