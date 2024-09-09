using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolComparerEvent : MonoBehaviour
{
    [SerializeField, ReadOnly] bool BoolToCompare = false;

    [SerializeField] UnityEvent<bool> OnBoolReceiveTrue;
    [SerializeField] UnityEvent<bool> OnBoolReceiveFalse;

    public void CompareBool(bool b)
    {
        if (b == BoolToCompare)
        {
            OnBoolReceiveTrue?.Invoke(b);
        }
        else
        {
            OnBoolReceiveFalse?.Invoke(b);
        }
    }

    public void SetBool(bool b)
    {
        BoolToCompare = b;
    }
}
