using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOverDelegator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<PointerEventData, GameObject> E_OnPointerEntered = null;
    public event Action<PointerEventData, GameObject> E_OnPointerExitted = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        E_OnPointerEntered?.Invoke(eventData, gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        E_OnPointerExitted?.Invoke(eventData, gameObject);
    }
}