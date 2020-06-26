using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uxp_RewardsManager: MonoBehaviour {

	private new string name = "";
	private string rewardInfo = "";
	private int rank = 0;
	private int reward = 0;
	[HideInInspector]
	public int rewardAmount;
	public List<string> rewardNameList = new List<string>();
	public List<string> rewardDescriptionList = new List<string>();
	public List<int> rewardRankList = new List<int>();
	public List<int> rewardAmountList = new List<int>();
	public static List<string> rewardActiveList = new List<string>();

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		rewardActiveList = rewardNameList;
	}

	//Here we add our new rewards created using the Reward Editor
	public void AddReward (string _name, string _rewardInfo, int _rank, int _reward) {

		name = _name;
		rewardInfo = _rewardInfo;
		rank = _rank;
		reward = _reward;
		rewardNameList.Add (name);
		rewardDescriptionList.Add (rewardInfo);
		rewardRankList.Add (rank);
		rewardAmountList.Add (reward);
		PlayerPrefs.SetInt (name, 0);

	}

	//Here we check to see if we have unlocked a new reward, If we have we pass it on to our SessionRewards class
	public void CheckForReward(){

		XpManager.RefeshRanks ();
	
		foreach (int earnedReward in rewardRankList) {

			if(earnedReward <= XpManager.curRank){

				if (rewardRankList.Contains (earnedReward)) {

					int indexValue = earnedReward;
					int indexResult = rewardRankList.IndexOf (indexValue);
					//Debug.Log (earnedReward);

					string tempRewardName = rewardNameList [indexResult];
					string tempRewardDescription = rewardDescriptionList [indexResult];
					int tempRewardAmount = rewardAmountList [indexResult];

					int hasReward = PlayerPrefs.GetInt (tempRewardName, 0);

					if (hasReward == 1) {
				
						rewardAmount = 0;

					} else {
				
						PlayerPrefs.SetInt (tempRewardName, 1);
						rewardAmount += tempRewardAmount;
						uxp_SessionRewards.rewardPopupList.Add (tempRewardName);
						uxp_SessionRewards.rewardInfoList.Add (tempRewardDescription);

					}
				} else {
					rewardAmount = 0;
				}
			}
		}
	}

	// Here we remove rewards that have been chosen using the Reward Editor
	public void RemoveReward(int index){

		PlayerPrefs.DeleteKey (rewardNameList[index]);
		rewardNameList.RemoveAt (index);
		rewardDescriptionList.RemoveAt (index);
		rewardAmountList.RemoveAt (index);
		rewardRankList.RemoveAt (index);

	}

	//Here we reset our active rewards so they can be unlocked now the game has been reset
	public static void ResetActiveRewards(){

		foreach (string activeReward in rewardActiveList) {
			
			PlayerPrefs.SetInt(activeReward, 0);
		}
	}
}