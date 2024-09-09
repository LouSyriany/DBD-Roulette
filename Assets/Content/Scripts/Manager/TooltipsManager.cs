using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipsManager : MonoBehaviour
{
    public static TooltipsManager Instance { get; private set; }

    [SerializeField] RectTransform Tr;
    [SerializeField] TMPro.TextMeshProUGUI Txt;

    [ReadOnly] public Slot Hovering;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (!Tr) return;

        SetPosition();

        SetHovered();
    }

    void SetPosition()
    {
        Vector2 mouse = Input.mousePosition;

        mouse.x -= Screen.width / 2;
        mouse.y -= Screen.height / 2;

        Tr.anchoredPosition = mouse;
    }

    void SetHovered()
    {
        if (Hovering)
        {
            if (Hovering.GetType() == typeof(CharacterSlot))
            {
                Tr.gameObject.SetActive(false);
            }

            if (Hovering.GetType() == typeof(AddonSlot))
            {
                Tr.gameObject.SetActive(true);

                AddonSlot addonslot = Hovering as AddonSlot;

                Txt.text = addonslot.Equipable.Name;
            }

            if (Hovering.GetType() == typeof(ItemSlot))
            {
                Tr.gameObject.SetActive(true);

                ItemSlot itemSlot = Hovering as ItemSlot;

                Txt.text = itemSlot.Item.Name;
            }
        }
        else
        {
            Tr.gameObject.SetActive(false);
        }
    }
}
