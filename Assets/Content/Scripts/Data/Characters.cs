using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Characters")]
public class Characters : ScriptableObject
{
    public enum CharacterType { Killers, Survivors}

    public CharacterType Type;

    public Sprite Icon;
    [ReadOnly] public string Name = "";

    [ReadOnly] public string ID = "";

    public List<Perks> UniquePerks;

    public List<Addons> KillerAddons;

    public void SetName(string name)
    {
        ID = name;

        switch (Type)
        {
            case CharacterType.Killers:

                Name = name;
                Name = Name.Substring(4);

                Name = Name.Insert(3, " ");

                break;

            case CharacterType.Survivors:

                Name = name;
                Name = Name.Substring(4);

                string newName = "";

                for (int i = 0; i < Name.Length; i++)
                {
                    if (char.IsUpper(Name[i]))
                    {
                        if (i != 0 && Name[i - 1] != '-')
                        {
                            newName += " ";
                        }
                    }

                    newName += Name[i];
                }

                Name = newName;

                break;
        }
    }
}
