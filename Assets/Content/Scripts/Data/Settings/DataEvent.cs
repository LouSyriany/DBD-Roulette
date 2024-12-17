using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataEvent : MonoBehaviour
{
    [SerializeField] SettingsScriptable Data;

    [Space(10)]

    [SerializeField] UnityEvent<bool> OnBoolSettingUpdated;
    [SerializeField] UnityEvent<float> OnFloatSettingUpdated;
    [SerializeField] UnityEvent<int> OnIntSettingUpdated;
    [SerializeField] UnityEvent<string> OnStringSettingUpdated;

    void OnEnable()
    {
        DatasManagerV2.Instance.OnSettingUpdated += SendEvent;

        SendEvent(Data);
    }

    void OnDisable()
    {
        DatasManagerV2.Instance.OnSettingUpdated -= SendEvent;
    }

    void SendEvent(SettingsScriptable scriptable)
    {
        if (scriptable == Data)
        {
            if (Data is BoolData)OnBoolSettingUpdated?.Invoke(DatasManagerV2.Instance.GetSetting(Data as BoolData));

            if (Data is FloatData) OnFloatSettingUpdated?.Invoke(DatasManagerV2.Instance.GetSetting(Data as FloatData));

            if (Data is IntData) OnIntSettingUpdated?.Invoke(DatasManagerV2.Instance.GetSetting(Data as IntData));

            if (Data is StringData) OnStringSettingUpdated?.Invoke(DatasManagerV2.Instance.GetSetting(Data as StringData));
        }
    }
}
