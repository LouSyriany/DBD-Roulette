using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerV2 : MonoBehaviour
{
    [SerializeField] GameObject KillerRef;
    [SerializeField] GameObject SurvivorRef;
    [SerializeField] GameObject ItemRef;

    [Space(10)]

    [SerializeField] RectTransform KillerOptions;
    [SerializeField] RectTransform SurvivorOptions;
    [SerializeField] RectTransform ItemOptions;

    void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        List<RectTransform> rcts = new List<RectTransform>();

        if (KillerOptions && KillerRef)
        {
            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Killers.Count; i++)
            {
                var current = DatasManagerV2.Instance.DataBase.Killers[i];

                GameObject newKiller = Instantiate(KillerRef, KillerOptions);

                rcts.Add(newKiller.GetComponent<RectTransform>());

                newKiller.GetComponent<CharacterUnlockedItem>().Character = current.Ref as Characters;
                newKiller.name = current.Ref.name;
                newKiller.gameObject.SetActive(true);
            }
            SetRect(rcts, 3, new Vector2(1, 1));
        }

        if (SurvivorOptions && SurvivorRef)
        {
            rcts = new List<RectTransform>();

            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Survivors.Count; i++)
            {
                var current = DatasManagerV2.Instance.DataBase.Survivors[i];

                GameObject newSurvivor = Instantiate(SurvivorRef, SurvivorOptions);

                rcts.Add(newSurvivor.GetComponent<RectTransform>());

                newSurvivor.GetComponent<CharacterUnlockedItem>().Character = current.Ref as Characters;
                newSurvivor.name = current.Ref.name;
                newSurvivor.gameObject.SetActive(true);
            }
            SetRect(rcts, 3f, new Vector2(1, .5f));
        }

        if (ItemOptions && ItemRef)
        {
            rcts = new List<RectTransform>();

            List<ItemUnlocked> alreadyCreated = new List<ItemUnlocked>();

            for (int i = 0; i < DatasManagerV2.Instance.DataBase.Items.Count; i++)
            {
                Items cuItem = DatasManagerV2.Instance.DataBase.Items[i].Ref as Items;

                bool found = false;

                foreach (var item in alreadyCreated)
                {
                    if (item.Items[0].Type == cuItem.Type)
                    {
                        item.Items.Add(cuItem);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    GameObject newItems = Instantiate(ItemRef, ItemOptions);

                    rcts.Add(newItems.GetComponent<RectTransform>());

                    ItemUnlocked iu = newItems.GetComponent<ItemUnlocked>();
                    iu.Items.Add(cuItem);
                    newItems.name = cuItem.Type.ToString();
                    alreadyCreated.Add(iu);
                }
            }

            foreach (var item in alreadyCreated)
            {
                item.gameObject.SetActive(true);
            }

            SetRect(rcts, 2f, new Vector2(1, .5f));
        }
    }

    void SetRect(List<RectTransform> rcts, float divider, Vector2 ratio)
    {
        int line = 1;
        int column = 0;

        for (int i = 0; i < rcts.Count; i++)
        {
            rcts[i].anchorMin = new Vector2(column / divider, 1 - (ratio.y / divider) * line);
            rcts[i].anchorMax = new Vector2((column + 1.0f) / divider, 1 - (ratio.y / divider) * line + (ratio.y / divider));

            column++;

            if (column > divider - 1)
            {
                column = 0;
                line++;
            }
        }
    }
}
