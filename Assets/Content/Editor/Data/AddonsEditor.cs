using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Addons))]
public class AddonsEditor : Editor
{
    Addons addon;

    void OnEnable()
    {
        addon = target as Addons;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (addon.Icon == null) return;

        Texture2D textureIcon = AssetPreview.GetAssetPreview(addon.Icon);

        Texture2D textureBg = (Texture2D)AssetDatabase.LoadAssetAtPath(Rarity.GetAddonRarity(addon.RarityType), typeof(Texture2D));
        
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
