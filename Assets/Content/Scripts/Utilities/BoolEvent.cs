using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolEvent : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> OnBoolReceiveTrue;
    [SerializeField] UnityEvent<bool> OnBoolReceiveFalse;

    public void CompareBool(bool b)
    {
        if (b)
        {
            OnBoolReceiveTrue?.Invoke(b);
        }
        else
        {
            OnBoolReceiveFalse?.Invoke(b);
        }
    }
}
