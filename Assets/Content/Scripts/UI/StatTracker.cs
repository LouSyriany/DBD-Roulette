using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(+1)]
public class StatTracker : MonoBehaviour
{
    enum StatToTrack { SurvivorRoll, KillerRoll, EscapeRate, KillRate }

    [SerializeField] StatToTrack StatTracked;

    [SerializeField] TextMeshProUGUI Stat;
    [SerializeField] Image Graph;

    void OnEnable()
    {
        StatsManager.Instance.OnDataUpdated += UpdateStat;

        UpdateStat();
    }

    void OnDisable()
    {
        StatsManager.Instance.OnDataUpdated -= UpdateStat;
    }

    void UpdateStat()
    {
        if (!StatsManager.Instance) return;

        float percent = 0f;

        switch (StatTracked)
        {
            case StatToTrack.SurvivorRoll:
                percent = StatsManager.Instance.Stats.survivorPercent;
                break;

            case StatToTrack.KillerRoll:
                percent = StatsManager.Instance.Stats.killerPercent;
                break;

            case StatToTrack.EscapeRate:
                percent = (float)StatsManager.Instance.Items.SurvivorVictory / (float)StatsManager.Instance.Stats.rollMade * 100f;
                break;

            case StatToTrack.KillRate:
                percent = (float)StatsManager.Instance.Items.KillerVictory / (float)StatsManager.Instance.Stats.rollMade * 100f;
                break;
        }

        if (float.IsNaN(percent)) percent = 0;

        Stat.text = percent.ToString("n2") + "%";
        Graph.fillAmount = percent / 100;
    }
}
