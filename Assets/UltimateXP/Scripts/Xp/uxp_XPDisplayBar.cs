using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uxp_XPDisplayBar : MonoBehaviour {

	public Slider xpProgressBar;

	// Use this for initialization
	void Start () {

		UpdateXpBar ();

	}

	//Here we update our xp bar to reflect our current xp amount
	void UpdateXpBar(){

		xpProgressBar.value = PlayerPrefs.GetInt ("CurXP");
	}

	//Here we keep our xp bar upto date with our current progress
	void Update(){

		xpProgressBar.maxValue = XpManager.xpBarValue;
		xpProgressBar.value = PlayerPrefs.GetInt ("CurXP");

	}
}
