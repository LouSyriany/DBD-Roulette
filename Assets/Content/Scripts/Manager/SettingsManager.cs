using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum SettingBoolType 
    { 
        Addons, 
        Perks, 
        Items, 
        Characters, 
        RarityRoll, 
        CharacterStreak, 
        PerksStreak,
        AddonsDud,
        PerksDud,
        ItemsDud
    }

    public enum SettingIntType
    {
        AddonsDud,
        PerksDud,
        ItemsDud
    }

    [Serializable]
    public class SettingsBoolParameters
    {
        public SettingBoolType Setting;
        public Toggle Toggle;
    }

    [Serializable]
    public class SettingsIntParameters
    {
        public SettingIntType Setting;
        public SlidderFill Slidder;
    }

    public List<SettingsBoolParameters> SettingsBool = new List<SettingsBoolParameters>();

    public List<SettingsIntParameters> SettingsInt = new List<SettingsIntParameters>();
}
