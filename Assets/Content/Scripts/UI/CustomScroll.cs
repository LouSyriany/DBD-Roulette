using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScroll : MonoBehaviour
{
    [SerializeField] RectTransform Viewport;

    [SerializeField] float Speed = 5;

    [Space(10)]

    [SerializeField] float min = 0;
    public float Max = 1;

    RectTransform tr;

    float ratio;

    Vector3 mouseStartPos;

    bool mouseClicked = false;

    Vector2 startA = Vector2.zero;
    Vector2 startB = Vector2.zero;

    private void OnEnable()
    {
        ratio = Screen.width / Screen.height;
        if (!tr)
        {
            tr = GetComponent<RectTransform>();
        }

        startA = tr.anchorMin;
        startB = tr.anchorMax;
    }

    void Update()
    {
        if (!tr) return;

        if (mouseClicked || RectTransformUtility.RectangleContainsScreenPoint(Viewport, Input.mousePosition))
        {
            Vector2 tmp = tr.anchorMin;

            float scroll = Input.mouseScrollDelta.y;


            if (Input.GetMouseButtonDown(2))
            {
                mouseStartPos = Input.mousePosition;
                mouseClicked = true;
            }

            if (Input.GetMouseButton(2))
            {
                Vector2 v = Input.mousePosition - mouseStartPos;

                scroll += v.normalized.y * v.magnitude / 100;
            }

            tmp.y -= scroll * Time.deltaTime * Speed * ratio;

            tmp.y = Mathf.Clamp(tmp.y, min, Max);

            tr.anchorMin = tmp;

            tmp.x = 1;
            tmp.y += 1;

            tr.anchorMax = tmp;
        }

        if (Input.GetMouseButtonUp(2))
        {
            mouseClicked = false;
        }
    }

    public void SetMaxValue(float f)
    {
        Max = f;
    }

    public void ResetScroll()
    {
        if (tr)
        {
            tr.anchorMin = startA;
            tr.anchorMax = startB;
        }
    }
}
