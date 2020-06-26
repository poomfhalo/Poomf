using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uxp_XPFeedText : MonoBehaviour {

	public uxp_ShowXp showXP;
	private float delay = 2f;
	private Text xpText;

	public void SetText(string text)
    {
	    showXP.xpScore = text;
		StartCoroutine (ClearXPDisplay());
    }

	public IEnumerator ClearXPDisplay(){

		yield return new WaitForSeconds (delay);
		Destroy (gameObject);

	}
}
