using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image Icon;
    public Image BG;

    public Color KillerColor = new Color(0.475f, 0.116f, 0.152f);
    public Color SurvivorColor = new Color(0.384f, 0.464f, 0.518f);
    public Color OffColor = new Color(0.59f, 0.06f, 0.06f, 0.42f);

    RectTransform tr;

    void Awake()
    {
        tr = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!TooltipsManager.Instance) return;
        if (tr == null) return;

        if(RectTransformUtility.RectangleContainsScreenPoint(tr, Input.mousePosition) && gameObject.activeSelf)
        {
            TooltipsManager.Instance.Hovering = this;
        }
        else
        {
            if(TooltipsManager.Instance.Hovering == this)
            {
                TooltipsManager.Instance.Hovering = null;
            }
        }
    }
}
