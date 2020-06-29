using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleButtonGroup : MonoBehaviour
{
    public ToggleButton ActiveButton => activeButton;
    [SerializeField] ToggleButton activeButton = null;

    List<ToggleButton> toggleButtons = new List<ToggleButton>();
    void Start()
    {
        toggleButtons = GetComponentsInChildren<ToggleButton>().ToList();
        foreach (var t in toggleButtons)
        {
            t.OnSelected += OnSelected;
        }
        if (activeButton)
        {
            activeButton.Select();
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