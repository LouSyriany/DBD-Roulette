using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Perks))]
public class PerksEditor : Editor
{
    Perks perk;

    void OnEnable()
    {
        perk = target as Perks;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (perk.Icon == null) return;

        Texture2D textureIcon = AssetPreview.GetAssetPreview(perk.Icon);

        Texture2D textureBg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Content/Textures/Misc/Perks_Bg_Veryrare.png", typeof(Texture2D));

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.Label("", GUILayout.Height(100), GUILayout.Width(100));

        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), textureBg);
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), textureIcon);

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }
}
