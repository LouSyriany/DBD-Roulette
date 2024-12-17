using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlidderFill : MonoBehaviour
{
    [SerializeField] int MinValue = 0;
    [SerializeField] int MaxValue = 100;

    public int currentValue = 50;

    [SerializeField] string Suffixe = "";

    [SerializeField] Image Fill;
    [SerializeField] TMPro.TextMeshProUGUI Text;

    [SerializeField] UnityEvent<int> OnValueChange;

    int increment = 1;
    int multiplier = 1;

    void OnEnable()
    {
        Setup();

        OnValueChange?.Invoke(currentValue);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            multiplier = 10;
        }
        else
        {
            multiplier = 1;
        }
    }

    void Setup()
    {
        float sliderValue = currentValue / (float)MaxValue;

        if (Fill) Fill.rectTransform.anchorMax = new Vector2(sliderValue, 1);

        if (Text) Text.text = currentValue.ToString() + Suffixe;
    }

    public void ChangeValue(bool up)
    {
        if (up) 
        {
            currentValue += increment * multiplier;
        }
        else
        {
            currentValue -= increment * multiplier;
        }

        currentValue = Mathf.Clamp(currentValue, MinValue, MaxValue);

        Setup();

        OnValueChange?.Invoke(currentValue);
    }

    public void SetValue(int i)
    {
        currentValue = i;

        Setup();

        OnValueChange?.Invoke(currentValue);
    }
}
