using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AfterAnimationDelay : MonoBehaviour
{
    [SerializeField] AnimationClip Animation;

    [SerializeField] float ExtraTime = 0;

    [SerializeField] bool StartOnEnable = true;

    [SerializeField] UnityEvent OnAnimationDelay;

    IEnumerator IE_waiting;
    
    void OnEnable()
    {
        if(StartOnEnable) StartDelay();
    }
    public void StartDelay()
    {
        if (IE_waiting == null)
        {
            StartCoroutine(IE_waiting = StartWait());
        }
    }

    IEnumerator StartWait()
    {
        yield return new WaitForSeconds(Animation.length + ExtraTime);

        OnAnimationDelay?.Invoke();

        IE_waiting = null;
    }
}
