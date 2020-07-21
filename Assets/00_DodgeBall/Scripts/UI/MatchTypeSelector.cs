using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTypeSelector : MonoBehaviour
{
    ToggleButtonGroup buttonsGroup = null;

    void Start()
    {
        buttonsGroup = GetComponent<ToggleButtonGroup>();
        buttonsGroup.onActiveButtonUpdated += OnActiveButtonUpdated;
    }
    void OnDestroy()
    {
        if(buttonsGroup)
            buttonsGroup.onActiveButtonUpdated -= OnActiveButtonUpdated;
    }
    public bool GetMatchType(out MatchType matchType)
    {
        if (!buttonsGroup.ActiveButton)
        {
            matchType = MatchType.Practice;
            return false;
        }
        MatchTypeButton typeButton = buttonsGroup.ActiveButton.GetComponent<MatchTypeButton>();

        matchType = typeButton.matchType;
        return true;
    }
    private void OnActiveButtonUpdated()
    {

    }
    
    public void SetInteractable(bool state)
    {
        buttonsGroup.SetInteractable(state);
    }
}