using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemBase : MonoBehaviour
{
    [SerializeField] private Button defaultMenuButton = null;

    public Button GetDefaultButton()
    {
        if (null == defaultMenuButton)
            Debug.LogError("MenuItemBase::GetDefaultButton -> Default button was not assigned.");

        return defaultMenuButton;
    }
}
