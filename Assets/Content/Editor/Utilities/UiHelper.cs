using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UiHelper : EditorWindow
{
    [MenuItem("UiHelper/HelperWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UiHelper));
    }

    int divider = 3;

    bool startBtm = true;

    void OnGUI()
    {
        divider = EditorGUILayout.IntField("Dividers :", divider);
        startBtm = EditorGUILayout.Toggle("Start at bottom :", startBtm);

        GUILayout.Space(10);

        if (GUILayout.Button("Arrange Vertical"))
        {
            if (divider != 0)
            {
                ArrangeVertical();
            }
            else
            {
                Debug.LogError("Cannot divide by 0");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Arrange Horizontal"))
        {
            if (divider != 0)
            {
                ArrangeHorizontal();
            }
            else
            {
                Debug.LogError("Cannot divide by 0");
            }
        }
    }

    void ArrangeHorizontal()
    {
        Object[] slct = Selection.objects;

        if (startBtm)
        {
            int index = 0;

            for (int i = 0; i < slct.Length; i++)
            {
                GameObject go = slct[i] as GameObject;

                RectTransform tr = go.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(index / (float)divider, 0);
                tr.anchorMax = new Vector2((index + 1.0f) / (float)divider, 1);

                index++;
            }
        }
        else
        {
            int index = 1;

            for (int i = 0; i < slct.Length; i++)
            {
                GameObject go = slct[i] as GameObject;

                RectTransform tr = go.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(1 - index * (1 / (float)divider), 0);
                tr.anchorMax = new Vector2(1 - (index - 1) * (1 / (float)divider), 1);

                index++;
            }
        }
    }

    void ArrangeVertical()
    {
        Object[] slct = Selection.objects;

        if (startBtm)
        {
            int index = 0;

            for (int i = 0; i < slct.Length; i++)
            {
                GameObject go = slct[i] as GameObject;

                RectTransform tr = go.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(0, index / (float)divider);
                tr.anchorMax = new Vector2(1, (index + 1.0f) / (float)divider);

                index++;
            }
        }
        else
        {
            int index = 1;

            for (int i = 0; i < slct.Length; i++)
            {
                GameObject go = slct[i] as GameObject;

                RectTransform tr = go.GetComponent<RectTransform>();

                tr.anchorMin = new Vector2(0, 1 - index * (1 / (float)divider));
                tr.anchorMax = new Vector2(1, 1 - (index - 1) * (1 / (float)divider));

                index++;
            }
        }
    }
}
