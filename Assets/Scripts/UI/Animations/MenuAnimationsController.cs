using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationsController : MonoBehaviour, IUIAnimationsController
{
    [SerializeField] Dictionary<string, IUIAnimatedScreenController> screens = new Dictionary<string, IUIAnimatedScreenController>();

    public void RegisterScreen(string screenID, IUIAnimatedScreenController screen)
    {
        screens.Add(screenID, screen);
    }

    #region IUIAnimationsController
    public void ShowScreen(string screenID)
    {
        StartCoroutine(screens[screenID].AnimateIn());
    }
    public void HideScreen(string screenID)
    {
        StartCoroutine(screens[screenID].AnimateOut());
    }
    #endregion
}
