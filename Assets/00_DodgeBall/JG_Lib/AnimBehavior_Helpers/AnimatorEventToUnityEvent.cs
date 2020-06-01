using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEventToUnityEvent : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent = null;

    //Called from Animator
    public void A_CallNext()
    {
        unityEvent?.Invoke();
    }
}