using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles xp gained by the player. It also takes into account if we are using rewards
public class uxp_XP : MonoBehaviour {

	uxp_SessionXP sessionXP;
	uxp_RewardsManager rewardsManager;
	public int xpAmount;
	public int xpMultiplier;

	// Here we find our sessionXP and rewardsManager
	void Start () {

		sessionXP = FindObjectOfType<uxp_SessionXP> ();
		rewardsManager = FindObjectOfType<uxp_RewardsManager> ();
	}

	// This is where we manage how much xp to award the player
	public void XpReward(){

		if (GameObject.Find("RewardsManagerSyncObject") == null) {

			Debug.LogError ("RewardsManagerSyncObject missing, You will not be able to earn rewards without it. Make sure you always start a level through your main menu first and that the RewardsManagerSyncObject is in the menu scene");
			sessionXP.xpAmount = xpAmount;
			sessionXP.xpMultiplier = xpMultiplier;
			sessionXP.AddXP ();

		} else {

			rewardsManager.CheckForReward ();
			sessionXP.xpAmount = xpAmount += rewardsManager.rewardAmount;
			sessionXP.xpMultiplier = xpMultiplier;
			sessionXP.AddXP ();

		}
	}
}
