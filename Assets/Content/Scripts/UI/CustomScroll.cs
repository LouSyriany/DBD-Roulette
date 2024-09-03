using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScroll : MonoBehaviour
{
    [SerializeField] float Speed = 5;

    [Space(10)]

    [SerializeField] float min = 0;
    [SerializeField] float max = 1;

    RectTransform tr;

    float ratio;

    Vector3 mouseStartPos;

    private void OnEnable()
    {
        ratio = Screen.width / Screen.height;
        if (!tr)
        {
            tr = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (!tr) return;

        Vector2 tmp = tr.anchorMin;

        float scroll = Input.mouseScrollDelta.y;


        if (Input.GetMouseButtonDown(2))
        {
            mouseStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector2 v = Input.mousePosition - mouseStartPos;

            scroll += v.normalized.y * v.magnitude / 100;
        }

        tmp.y -= scroll * Time.deltaTime * Speed * ratio;

        tmp.y = Mathf.Clamp(tmp.y, min, max);

        tr.anchorMin = tmp;

        tmp.x = 1;
        tmp.y += 1;

        tr.anchorMax = tmp;
    }

    public void SetMaxValue(float f)
    {
        max = f;
    }

}
