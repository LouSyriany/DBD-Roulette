using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseButtons : MonoBehaviour
{
    [SerializeField] bool StreakMode = false;

    [SerializeField] Button WinButton;
    [SerializeField] Button LoseButton;

    [SerializeField] Image WinImage;
    [SerializeField] Image LoseImage;

    [SerializeField] Sprite[] Sprites;

    [SerializeField] BoolData BoolData;

    RouletteManager.Result lastResult;

    bool mode = false;

    void Start()
    {
        if (!StreakMode)
        {
            WinButton.gameObject.SetActive(false);
            LoseButton.gameObject.SetActive(false);
        }
        
    }

    void OnEnable()
    {
        gameObject.SetActive(DatasManagerV2.Instance.GetSetting(BoolData));


        mode = StreakMode;

        if (mode)
        {
            Setup(RouletteManager.Instance.Results);
        }
        else
        {
            RouletteManager.Instance.OnRollMade += Setup;
        }
    }

    void OnDisable()
    {
        if (!mode)
        {
            RouletteManager.Instance.OnRollMade -= Setup;
        }
    }

    void Setup(RouletteManager.Result result)
    {
        lastResult = result;
        if (!WinButton.isActiveAndEnabled) WinButton.gameObject.SetActive(true);
        if (!LoseButton.isActiveAndEnabled) LoseButton.gameObject.SetActive(true);

        WinButton.interactable = true;
        LoseButton.interactable = true;

        WinImage.color = WinButton.image.color;
        LoseImage.color = LoseButton.image.color;

        if (Sprites.Length == 2)
        {
            if (result.Character.Type == Characters.CharacterType.Killers)
            {
                WinImage.sprite = Sprites[0];
                LoseImage.sprite = Sprites[1];
            }
            else
            {
                WinImage.sprite = Sprites[1];
                LoseImage.sprite = Sprites[0];
            }
        }
    }

    public void SendOutcome(bool isVictory)
    {
        WinButton.interactable = false;
        LoseButton.interactable = false;

        WinImage.color = WinButton.image.color * new Color(1,1,1, 0.2509804f);
        LoseImage.color = LoseButton.image.color * new Color(1, 1, 1, 0.2509804f);

        StatsManager.Instance.UpdateVictories(lastResult, isVictory);
    }
}
