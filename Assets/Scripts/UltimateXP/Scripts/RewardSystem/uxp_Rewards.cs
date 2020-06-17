using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class handles our rewards that the player has earned
public class uxp_Rewards : MonoBehaviour {

	[HideInInspector]
	public List<string> rewardList = new List<string>();
	[HideInInspector]
	public List<string> rewardInfoList = new List<string>();
	private int rewardIndex = 0;
	public GameObject rewardPopup;
	public Text rewardText;
	public Text rewardInfoText;

	// Here we grab our reward names and information from our sessionRewards class, If no rewards have been earned we simply return out of the method
	void Start () {

		rewardList =  uxp_SessionRewards.rewardPopupList;
		rewardInfoList = uxp_SessionRewards.rewardInfoList;
		if (rewardList.Count == 0) {
			return;
		} else {
			RewardPopup ();
		}
	}

	// Here we display any rewards that have been earned. We also set out blur effect to define our popup to the user
	public void RewardPopup(){

		uxp_XPDisplayManager xpDisplayManager = GetComponent<uxp_XPDisplayManager>();
		xpDisplayManager.SetBlurEffect ();
		rewardPopup.SetActive (true);
		rewardText.text = rewardList [rewardIndex];
		rewardInfoText.text = rewardInfoList [rewardIndex];
	}

	// Here we close our popup and move to the next one in the list, If no more rewards have been earned we clear the reward list and close the popup
	public void CloseRewardPopup(){

		if (rewardList.Count == 1){
			rewardList.RemoveAt (rewardIndex);
			rewardInfoList.RemoveAt (rewardIndex);
			rewardPopup.SetActive (false);
			uxp_XPDisplayManager xpDisplayManager = GetComponent<uxp_XPDisplayManager>();
			xpDisplayManager.CloseBlurEffect ();
		} else {
			rewardList.RemoveAt (rewardIndex);
			rewardInfoList.RemoveAt (rewardIndex);
			RewardPopup ();
		}
	}
}
