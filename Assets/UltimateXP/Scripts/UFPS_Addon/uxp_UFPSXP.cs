using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uxp_UFPSXP : MonoBehaviour {

	uxp_XP xp;

	// Use this for initialization
	void Start () {

		xp = GetComponent <uxp_XP> ();
		uxp_XPFeedManager.Initialize ();
		
	}
	
	void Die(){

		xp.XpReward ();
		uxp_XPFeedManager.CreateXPText (xp.xpAmount.ToString (), transform);

	}
}
