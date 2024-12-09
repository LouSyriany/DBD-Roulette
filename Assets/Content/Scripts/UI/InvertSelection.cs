using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvertSelection : MonoBehaviour
{
    [SerializeField] GameObject SurvivorsOptionMenu;
    [SerializeField] GameObject KillersOptionMenu;
    [SerializeField] GameObject ItemsOptionMenu;
    [SerializeField] GameObject SettingsOptionMenu;


    public void InvertMain()
    {
        CharacterUnlockedItem[] CharacterUnlockedItems = FindObjectsByType<CharacterUnlockedItem>(FindObjectsSortMode.None);
        ItemUnlocked[] ItemUnlocked = FindObjectsByType<ItemUnlocked>(FindObjectsSortMode.None);


        foreach (var item in CharacterUnlockedItems)
        {
            if (SurvivorsOptionMenu.activeInHierarchy)
            {
                if (item.Character.Type == Characters.CharacterType.Survivors)
                {
                    item.InvertCharacter();
                }
            }

            if (KillersOptionMenu.activeInHierarchy)
            {
                if (item.Character.Type == Characters.CharacterType.Killers)
                {
                    item.InvertCharacter();
                }
            }
        }

        if (ItemsOptionMenu.activeInHierarchy)
        {
            foreach (var item in ItemUnlocked)
            {
                item.InvertItems();
            }
        }  
    }

    public void InvertPerks()
    {
        CharacterUnlockedItem[] CharacterUnlockedItems = FindObjectsByType<CharacterUnlockedItem>(FindObjectsSortMode.None);

        foreach (var item in CharacterUnlockedItems)
        {
            if (SurvivorsOptionMenu.activeInHierarchy)
            {
                if (item.Character.Type == Characters.CharacterType.Survivors)
                {
                    item.InvertPerks();
                }
            }

            if (KillersOptionMenu.activeInHierarchy)
            {
                if (item.Character.Type == Characters.CharacterType.Killers)
                {
                    item.InvertPerks();
                }
            }
        }
    }

    public void InvertAddons()
    {
        CharacterUnlockedItem[] CharacterUnlockedItems = FindObjectsByType<CharacterUnlockedItem>(FindObjectsSortMode.None);
        ItemUnlocked[] ItemUnlocked = FindObjectsByType<ItemUnlocked>(FindObjectsSortMode.None);


        foreach (var item in CharacterUnlockedItems)
        {
            if (KillersOptionMenu.activeInHierarchy)
            {
                if (item.Character.Type == Characters.CharacterType.Killers)
                {
                    item.InvertAddons();
                }
            }
        }

        if (ItemsOptionMenu.activeInHierarchy)
        {
            foreach (var item in ItemUnlocked)
            {
                item.InvertAddons();
            }
        }
    }
}
