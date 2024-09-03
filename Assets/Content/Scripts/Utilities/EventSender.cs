using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
    [SerializeField] UnityEvent OnEnableEvent;
    [SerializeField] UnityEvent OnStartEvent;
    [SerializeField] UnityEvent OnDisableEvent;

    void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    void Start()
    {
        OnStartEvent?.Invoke();
    }

    void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }
}
