using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple options class to enable features
public class uxp_UltimateXPOptions : MonoBehaviour {

	uxp_Rewards rewards;

	public bool useRequiredXP;
	public bool usePrestigeMode;
	public bool useRewardSystem;
	public bool useRankBadges;
	public bool usePromotionAnimations;
	public int SetMaxAllowedPrestiges;
	// option for blur effect

	// Use this for initialization
	void Start () {

		if (useRequiredXP) {

			XpManager.DisplayRequiredXP ();
		
		}
		if (usePrestigeMode) {

			XpManager.UsePrestigeMode ();

		}
		if (useRankBadges) {

			XpManager.UseRankBadges ();

		}
		if (!useRewardSystem) {
			
			rewards = GetComponent<uxp_Rewards> ();
			rewards.enabled = false;
		}
		if (usePromotionAnimations) {

			XpManager.UseRankAnimations ();
		}
		else
		{
			XpManager.UseRewardSystem ();
			rewards = GetComponent<uxp_Rewards> ();
			rewards.enabled = true;
		}
	}
}
