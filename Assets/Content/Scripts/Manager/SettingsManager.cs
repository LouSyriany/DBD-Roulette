using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Serializable]
    public class SettingsBoolParameters
    {
        [HideInInspector] public string name;
        public BoolData Data;
        public Toggle Toggle;
    }

    [Serializable]
    public class SettingsIntParameters
    {
        [HideInInspector] public string name;
        public IntData Data;
        public SlidderFill Slidder;
    }

    public List<SettingsBoolParameters> SettingsBool = new List<SettingsBoolParameters>();

    public List<SettingsIntParameters> SettingsInt = new List<SettingsIntParameters>();

    private void OnValidate()
    {
        foreach (var item in SettingsBool)
        {
            if (item.Data)
            {
                item.name = item.Data.name;
            }
        }

        foreach (var item in SettingsInt)
        {
            if (item.Data)
            {
                item.name = item.Data.name;
            }
        }
    }

    void OnEnable()
    {
        UpdateVisuals();

        DatasManagerV2.Instance.OnUserDataLoaded += UpdateVisuals;
    }

    void OnDisable()
    {
        DatasManagerV2.Instance.OnUserDataLoaded -= UpdateVisuals;
    }

    void UpdateVisuals()
    {
        foreach (var item in SettingsBool)
        {
            foreach (var data in DatasManagerV2.Instance.Settings.Bool)
            {
                if (item.Data == data.Ref)
                {
                    item.Toggle.isOn = data.BoolValue;
                }
            }
        }

        foreach (var item in SettingsInt)
        {
            foreach (var data in DatasManagerV2.Instance.Settings.Int)
            {
                if (item.Data == data.Ref)
                {
                    item.Slidder.SetValue(data.IntValue);
                }
            }
        }
    }

    public void UpdateValue(BoolData data)
    {
        foreach (var item in SettingsBool)
        {
            if (item.Data == data)
            {
                DatasManagerV2.Instance.UpdateSetting(data, item.Toggle.isOn);
            }
        }
    }

    public void UpdateValue(IntData data)
    {
        foreach (var item in SettingsInt)
        {
            if (item.Data == data)
            {
                DatasManagerV2.Instance.UpdateSetting(data, item.Slidder.currentValue);
            }
        }
    }
}
