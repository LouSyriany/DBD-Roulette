using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField, ReadOnly] int rollMade;

    [SerializeField, ReadOnly] int survivors;
    [SerializeField, ReadOnly] int killers;

    [SerializeField, ReadOnly] float survivorPercent;
    [SerializeField, ReadOnly] float killerPercent;

    void OnEnable()
    {
        RouletteManager.Instance.OnRollMade += AddData;
    }

    void OnDisable()
    {
        RouletteManager.Instance.OnRollMade -= AddData;
    }

    void AddData(RouletteManager.Result result)
    {
        rollMade += 1;

        if (result.Character.Type == Characters.CharacterType.Killers)
        {
            killers += 1;
        }
        else
        {
            survivors += 1;
        }

        UpdateData();
    }
    
    void UpdateData()
    {
        survivorPercent = (float)survivors / (float)rollMade * 100f;
        killerPercent = (float)killers / (float)rollMade * 100f;
    }





    [ContextMenu("Roll 500 Survivors")]
    void Roll500Survivors()
    {
        for (int i = 0; i < 500; i++)
        {
            RouletteManager.Instance.RollSurvivor();
        }
    }

    [ContextMenu("Roll 500 Killers")]
    void Roll500Killer()
    {
        for (int i = 0; i < 500; i++)
        {
            RouletteManager.Instance.RollKiller();
        }
    }

    [ContextMenu("Roll 500 Both")]
    void Roll500Both()
    {
        for (int i = 0; i < 500; i++)
        {
            RouletteManager.Instance.RollBoth();
        }
    }
}
