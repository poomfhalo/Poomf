using UnityEngine;
using TMPro;

public class CustomDebugText : GameDebuggable
{
    [SerializeField] TextMeshProUGUI customText = null;
    void Start()
    {
        if(customText == null)
        {
            customText = GetComponentInChildren<TextMeshProUGUI>();
        }
        customText.text = "";
    }

    public void AssignText(object o)
    {
        if (!customText)
            return;

        customText.text = o.ToString();
    }
    public void SetText(object o)
    {
        AssignText(o);
    }

    public override void SetActivity(bool toState)
    {
        if (!customText)
            return;

        customText.gameObject.SetActive(toState);
    }
}
