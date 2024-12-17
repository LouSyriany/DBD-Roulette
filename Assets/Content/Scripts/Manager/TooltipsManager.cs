using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipsManager : MonoBehaviour
{
    public static TooltipsManager Instance { get; private set; }

    [SerializeField] RectTransform Tr;
    [SerializeField] TMPro.TextMeshProUGUI Txt;

    [ReadOnly] public Slot Hovering;

    float mouseMovements = 0f;
    Vector3 prevMousePos = Vector2.zero;

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

        Vector3 mousePos = Input.mousePosition;

        Vector3 dist = prevMousePos - mousePos;

        mouseMovements = dist.magnitude;

        prevMousePos = mousePos;

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
        string instruction = "\n\n(Left Ctrl for extra data)";

        if (Hovering && mouseMovements <= 2f)
        {
            if (Hovering.GetType() == typeof(CharacterSlot))
            {
                Tr.gameObject.SetActive(true);

                CharacterSlot characterSlot = Hovering as CharacterSlot;

                Txt.text = Input.GetKey(KeyCode.LeftControl) ? GetStat(characterSlot.Character, characterSlot.Character.Name) : characterSlot.Character.Name + instruction;
            }

            if (Hovering.GetType() == typeof(AddonSlot))
            {
                Tr.gameObject.SetActive(true);

                AddonSlot addonslot = Hovering as AddonSlot;

                Txt.text = Input.GetKey(KeyCode.LeftControl) ? GetStat(addonslot.Equipable, addonslot.Equipable.Name) : addonslot.Equipable.Name + instruction;
            }

            if (Hovering.GetType() == typeof(ItemSlot))
            {
                Tr.gameObject.SetActive(true);

                ItemSlot itemSlot = Hovering as ItemSlot;

                Txt.text = Input.GetKey(KeyCode.LeftControl) ? GetStat(itemSlot.Item, itemSlot.Item.Name) : itemSlot.Item.Name + instruction;
            }
        }
        else
        {
            Tr.gameObject.SetActive(false);
        }
    }


    string GetStat(BaseScriptable Scriptable, string name)
    {
        int count = StatsManager.Instance.Items.GetCount(Scriptable);
        int win = StatsManager.Instance.Items.GetVictories(Scriptable);
        int lose = StatsManager.Instance.Items.GetLost(Scriptable);
        int roll = StatsManager.Instance.Stats.rollMade;

        float percent = (float)count / (float)roll * 100f;

        string text = name + '\n' + '\n';
        text += "Rolled : " + count + "/" + roll + "(" + percent.ToString("n2") + "%)" + '\n';
        text += "Won : " + win + '\n';
        text += "Lost : " + lose + '\n';

        return text;
    }
}
