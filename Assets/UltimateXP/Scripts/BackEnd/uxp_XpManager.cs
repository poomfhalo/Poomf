using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpManager {

	public static int xpIsIntialized;
	public static int targetXP;
	public static int curXP;
	public static int careerTotalXP;
	public static int requiredXP;
	public static int xpMultiplier;
	public static int aquiredXP;
	public static int xpCap;
	public static int curRank = 1;
	public static int nextRank = 2;
	public static int rankIndex;
	public static int nextRankIndex;
	public static float xpBarValue;
	public static int showNewRank;
	public static int catchedXP;
	public static int carriedXP;
	public static bool showRequiredXP;
	public static bool usingPrestigeMode;
	public static bool usingRewardSystem;
	public static bool usingRankBadges;
	public static bool usingRankAnimations;
	public static int inPrestigeMode;
	public static int prestigeLvl = 0;
	public static string rankIconPath;
	private static TextAsset rankData;
	private static string [] rankLadder;

	//Here we calculate our new xp earned
	public static void CalculateNewXP(int aquiredXP, int xpMultiplier){

		curXP = PlayerPrefs.GetInt ("CurXP", 0);
		curXP += aquiredXP * xpMultiplier;
		PlayerPrefs.SetInt ("CurXP", curXP);

		if (curXP >= xpCap) {
			PlayerPrefs.SetInt ("CurXP", xpCap);
		} 
	}

	public static void CalculateCareerXP(int aquiredXP, int xpMultiplier){
		careerTotalXP =  PlayerPrefs.GetInt ("CareerXP");
		careerTotalXP += aquiredXP * xpMultiplier;
		PlayerPrefs.SetInt ("CareerXP", careerTotalXP);
	}

	//Here we deduct our current xp from our target xp to see how much more xp is required to rank up. Our xp bar is also updated here
	public static void CalculateTargetXP(int targetXP){
		
		curXP = PlayerPrefs.GetInt ("CurXP", 0);
		xpBarValue = targetXP;
		int tempXP = PlayerPrefs.GetInt ("CurXP");
		if (tempXP == 0) {

			requiredXP = targetXP;
			PlayerPrefs.SetInt ("RequiredXP", requiredXP);

		}
		else{
			requiredXP = targetXP - tempXP;
			PlayerPrefs.SetInt ("RequiredXP", requiredXP);
			xpBarValue = targetXP;
			CheckRankProgress ();
		}
		//Here we make sure our gained xp isn't added in one big total thus skipping ranks.
		if (curXP >= targetXP) {
			catchedXP = targetXP;
			carriedXP = curXP; 
			carriedXP -= targetXP;
			curXP = catchedXP;
			PlayerPrefs.SetInt ("CurXP", curXP);
			if (curXP >= PlayerPrefs.GetInt ("MaxXpLevel")) {

			} else {
				
				PlayerPrefs.SetInt ("CarriedXP", carriedXP);
				CalculateNewRank ();
			}
		}
	}

	//Here we check to see if we are ready to move to the next rank
	public static void CheckRankProgress(){

		if (curXP >= xpCap) {

			if (curXP >= PlayerPrefs.GetInt ("MaxXpLevel")) {
				PlayerPrefs.SetInt ("CanPrestige", 1);
			}	
		}
	}

	//Here we set our new prestige rank
	public static void CalculateNewPrestige(){
		prestigeLvl = PlayerPrefs.GetInt ("CurPrestigeLvl", 0);
		prestigeLvl += 1;
		curXP = 0;
		curRank = 1;
		nextRank = 2;
		PlayerPrefs.SetString ("RankIconPath", "Ranks/Prestiges");
		PlayerPrefs.SetInt ("CurPrestigeLvl", prestigeLvl);
		PlayerPrefs.SetInt ("CurXP", 0);
		PlayerPrefs.SetInt ("Rank", 1);
		PlayerPrefs.SetInt ("NextRank", 2);
		PlayerPrefs.SetInt ("InPrestigeMode", 1);
		PlayerPrefs.SetInt ("CanPrestige", 0);
		rankData = Resources.Load<TextAsset> ("RankData/RankData");
		rankLadder= (rankData.text.Split( '\n' ));
		PlayerPrefs.SetString ("RankTitle", (rankLadder[1]));
		PlayerPrefs.SetString ("NextRankTitle", (rankLadder[2]));
		PlayerPrefs.SetInt ("CareerXP", 0);

	}

	//Here we calculate our new rank
	public static void CalculateNewRank(){

		curRank += 1;
		nextRank += 1;
		PlayerPrefs.SetInt ("Rank", curRank);
		PlayerPrefs.SetInt ("NextRank", nextRank);
		showNewRank = 1;
		PlayerPrefs.SetInt ("CurXP", carriedXP);
	}

	//Here we calculate our required xp
	public static void GetRankIndex(){

		inPrestigeMode = PlayerPrefs.GetInt ("InPrestigeMode");
		if (inPrestigeMode == 1) {
			rankIndex = PlayerPrefs.GetInt ("Rank");
			nextRankIndex = PlayerPrefs.GetInt ("NextRank");
		} else {
			rankIndex = PlayerPrefs.GetInt ("Rank");
			nextRankIndex = PlayerPrefs.GetInt ("NextRank");
		}
	}

	//Here we refesh our rank data to insure our data is current
	public static void RefeshRanks(){

		curRank = PlayerPrefs.GetInt ("Rank");
		nextRank = PlayerPrefs.GetInt ("NextRank");

	}

	public static void DisplayRequiredXP(){

		showRequiredXP = true;

	}

	public static void UsePrestigeMode(){

		usingPrestigeMode = true;

	}

	public static void UseRankBadges(){

		usingRankBadges = true;

	}

	public static void UseRewardSystem(){

		usingRewardSystem = true;

	}

	public static void UseRankAnimations(){

		usingRankAnimations = true;
	}

	public static void RankIconPath(){

		rankIconPath = PlayerPrefs.GetString ("RankIconPath");

	}


	//Here we reset player data
	public static void IntializeXPData () {

		xpIsIntialized = 1;
		PlayerPrefs.SetInt ("XPIntialized", xpIsIntialized);
		PlayerPrefs.SetInt ("Rank", 1);
		PlayerPrefs.SetInt ("NextRank", 2);
		rankData = Resources.Load<TextAsset> ("RankData/RankData");
		rankLadder= (rankData.text.Split( '\n' ));
		PlayerPrefs.SetString ("RankTitle", (rankLadder[1]));
		PlayerPrefs.SetString ("NextRankTitle", (rankLadder[2]));
		PlayerPrefs.SetInt ("CurXP", 0);
		PlayerPrefs.SetInt ("InPrestigeMode", 0);
		PlayerPrefs.SetInt ("CurPrestigeLvl", 0);
		PlayerPrefs.SetString ("RankIconPath", "Ranks");
		PlayerPrefs.SetInt ("CanPrestige", 0);
		PlayerPrefs.SetInt ("CareerXP", 0);
		PlayerPrefs.SetInt ("CarriedXP", 0);
		PlayerPrefs.SetInt ("NextPrestigeBadge", 1);
		uxp_RewardsManager.ResetActiveRewards ();
	}
}
