using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Items")]
public class Items : ScriptableObject
{
    public enum ItemType { Flashlights, Keys, Maps, MedKits, Toolboxes }

    public ItemType Type;

    public Rarity.RarityTypes Rarity;

    public Sprite Icon;
    [ReadOnly] public string Name = "";

    [Space(10)]

    [SerializeField] bool EditName = false;
    [SerializeField] string EditedName = "";

    public Addons[] ItemAddons;

    void OnValidate()
    {
        if (EditName)
        {
            Name = EditedName;
        }
        else
        {
            Name = name;

            string tmp = "";

            for (int i = 0; i < Name.Length; i++)
            {
                if (char.IsUpper(Name[i]))
                {
                    if (i != 0 && Name[i - 1] != '-')
                    {
                        tmp += " ";
                    }
                }

                tmp += Name[i];
            }

            Name = EditedName = tmp;
        }
    }
}
