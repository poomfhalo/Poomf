using UnityEngine;

public class RegionSelector : MonoBehaviour
{
    ToggleButtonGroup buttonsGroup = null;

    void Start()
    {
        buttonsGroup = GetComponent<ToggleButtonGroup>();
    }
    public void SetInteractable(bool toState)
    {
        buttonsGroup.SetInteractable(toState);
    }
    public bool GetRegion(out string region)
    {
        ToggleButton button = buttonsGroup.ActiveButton;
        if (button == null || button.text =="DEF")
        {
            region = "DEF";
            return false;
        }

        region = buttonsGroup.ActiveButton.text.ToLower();
        return true;
    }
}