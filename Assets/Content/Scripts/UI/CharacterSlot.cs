using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public Characters Character;

    [Space(10)]

    public Image Icon;
    public Image BG;

    [Space(10)]

    public Color KillerColor = new Color(0.475f, 0.116f, 0.152f);
    public Color SurvivorColor = new Color(0.384f, 0.464f, 0.518f);

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
