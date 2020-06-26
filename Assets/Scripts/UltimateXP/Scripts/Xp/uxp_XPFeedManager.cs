using UnityEngine;
using System.Collections;

public class uxp_XPFeedManager : MonoBehaviour {
	
	private static uxp_XPFeedText popupText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("UltimateXP_Canvas");
        if (!popupText)
			popupText = Resources.Load<uxp_XPFeedText>("PopupXPTextParent");
    }

    public static void CreateXPText(string text, Transform location)
    {

		uxp_XPFeedText instance = Instantiate(popupText);
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;
		instance.SetText (text);
    }
}
