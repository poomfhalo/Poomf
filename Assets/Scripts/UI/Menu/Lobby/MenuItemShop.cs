using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemShop : MenuItemBase
{
    [SerializeField] private MenuNavigator mainmenuNavigator = null;

    private void OnEnable()
    {
        if (null == mainmenuNavigator) return;
        mainmenuNavigator.enabled = false;
    }

    private void OnDisable()
    {
        if (null == mainmenuNavigator) return;
        mainmenuNavigator.enabled = true;
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }
}
