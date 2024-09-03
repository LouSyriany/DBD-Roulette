using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Items))]
public class ItemEditor : Editor
{
    Items item;

    void OnEnable()
    {
        item = target as Items;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (item.Icon == null) return;

        Texture2D textureIcon = AssetPreview.GetAssetPreview(item.Icon);

        Texture2D textureBg = (Texture2D)AssetDatabase.LoadAssetAtPath(Rarity.GetAddonRarity(item.Rarity), typeof(Texture2D));

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
