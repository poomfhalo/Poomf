using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uxp_ShowXp : MonoBehaviour {

	public Text xpDisplayScore;
	public string xpScore;

	//Here we update our current xp
	public void Update(){

		xpDisplayScore.text = ("+") + xpScore;

	}
}
