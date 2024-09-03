using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Version : MonoBehaviour
{
    TMPro.TextMeshProUGUI Text;

    void OnEnable()
    {
        if (!Text) Text = GetComponent<TMPro.TextMeshProUGUI>();

        if (Text)
        {
            Text.text = Application.version;
        }
    }
}
