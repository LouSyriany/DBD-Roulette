using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Characters))]
public class CharactersEditor : Editor
{
    Characters character;

    void OnEnable()
    {
        character = target as Characters;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (character.Icon == null) return;

        Texture2D textureIcon = AssetPreview.GetAssetPreview(character.Icon);

        Texture2D textureBg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Content/Textures/Misc/CharPortrait_CharPortrait.png", typeof(Texture2D));

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
