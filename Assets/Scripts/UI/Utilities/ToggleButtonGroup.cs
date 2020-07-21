using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleButtonGroup : MonoBehaviour
{
    public event Action onActiveButtonUpdated = null;
    public ToggleButton ActiveButton => activeButton;
    [SerializeField] GameObject inActivityImage = null;
    [SerializeField] List<ToggleButton> toggleButtons = new List<ToggleButton>();
    
    [Header("Read Only")]
    [SerializeField] ToggleButton activeButton = null;

    void Start()
    {
        if (toggleButtons.Count == 0)
        {
            toggleButtons = GetComponentsInChildren<ToggleButton>().ToList();
        }
        foreach (var t in toggleButtons)
        {
            t.OnSelected += OnSelected;
        }
        if (activeButton)
        {
            activeButton.Select();
        }
    }

    public void SetInteractable(bool state)
    {
        if(inActivityImage)
        {
            inActivityImage.SetActive(!state);
        }
        foreach (var b in toggleButtons)
        {
            b.SetInteractable(state);
        }
    }

    private void OnSelected(ToggleButton selection)
    {
        foreach (var t in toggleButtons)
        {
            if (t == selection)
                continue;

            t.DeSelect();
        }

        activeButton = selection;
    }
}