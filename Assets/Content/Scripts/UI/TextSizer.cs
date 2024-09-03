using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextSizer : MonoBehaviour
{
    [SerializeField] float Size = 15;

    TextMeshProUGUI Text;

    void OnEnable()
    {
        if (!Text) Text = GetComponent<TextMeshProUGUI>();

        SetSize();
    }

    void Update()
    {
        if(!Application.isPlaying) SetSize();
    }

    void SetSize()
    {
        if (Text)
        {
            Text.fontSize = Size * Screen.width;
        }
    }
}
