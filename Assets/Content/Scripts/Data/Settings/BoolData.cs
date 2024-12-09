using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolSetting", menuName = "ScriptableObjects/Data/Bool")]
public class BoolData : SettingsScriptable
{
    public bool DefaultValue = false;
}
