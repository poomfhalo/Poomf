using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class uxp_XPDisplayManager : MonoBehaviour {

	public PostProcessingBehaviour filters;
	uxp_UltimateXPOptions xpOptions;

	[Header("TEXT")]
	public Text curXPText;
	public Text reqXPText;
	public Text curRankText;
	public Text nextRankText;
	public Text curLvlText;
	[Header("XP")]
	private TextAsset xpData;
	private string [] xpLadder;
	private int targetXP;
	private int maxLevel;
	private int maxXp;
	private int xpCap;
	//private int nextPresitgeRank = 1;
	private int trackNextPrestigeBadge = 1;
	private int totalActualXP;
	public Slider xpProgressBar;
	[Header("BADGES")]
	public Image curRankBadgeLarge;
	public Image curRankBadge;
	public Image nextRankBadge;
	public GameObject nextRankBadgeObj;
	[Header("PRESTIGE")]
	public GameObject prestigeButton;
	public GameObject prestigePopup;
	[Header("AUDIO")]
	public AudioClip[] prestigeSounds;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {

		if (PlayerPrefs.GetInt ("XPIntialized") == 0) {

			XpManager.IntializeXPData ();
		}
			audioSource = GetComponent<AudioSource> ();
			xpOptions = GetComponent<uxp_UltimateXPOptions> ();
			LoadXPData ();
			UpdateXpBar ();
			UpdateTargetXP ();
			CheckMaxTotalXp ();
			CloseBlurEffect ();
	}

	//Here we load our xp data from our resources folder
	public void LoadXPData(){

		xpData = Resources.Load<TextAsset> ("XPData/XPData");
		xpLadder= (xpData.text.Split( '\n' ));
		maxXp = xpLadder.Length - 1;
		xpCap = System.Int32.Parse (xpLadder [maxXp]);
		PlayerPrefs.SetInt ("MaxXpLevel", xpCap);
	}

	//Here we calaculate how much actual xp is needed to max out and how much total xp is needed which is displayed to the player.
	public void CheckMaxTotalXp(){

		XpManager.xpCap = PlayerPrefs.GetInt ("CurXP");
		PlayerPrefs.GetInt ("CurXP");
		maxLevel =  xpLadder.Length - 1;
		PlayerPrefs.SetInt ("MaxLevel", maxLevel);
		PlayerPrefs.SetInt ("MaxTotalLevel", maxLevel +1);

		for (int i = 0; i  < xpLadder.Length; i++)
		{
			totalActualXP += System.Int32.Parse (xpLadder [i]);
		}
		PlayerPrefs.SetInt ("MaxTotalXP", totalActualXP);
	}

	//Here we update our xp bar to reflect our current xp amount
	void UpdateXpBar(){

		xpProgressBar.value = PlayerPrefs.GetInt ("CurXP");
	}

	//Here we open the prestige popup and set the blur effect. "Blur can be disabled from the UltimateXP Options script"
	public void OpenPrestigePopup(){

		audioSource.PlayOneShot(prestigeSounds[0], 0.7F);
		prestigePopup.SetActive (true);
		SetBlurEffect ();
	}

	//Here we manage which prestige the player has moved to and manage the ui elements.
	public void EnterPrestigeMode(){
		
		audioSource.PlayOneShot(prestigeSounds[1], 0.7F);
		XpManager.CalculateNewPrestige ();
		prestigeButton.SetActive (false);
		prestigePopup.SetActive (false);
		PlayerPrefs.GetInt ("NextPrestigeBadge");
		trackNextPrestigeBadge += 1;
		PlayerPrefs.SetInt ("NextPrestigeBadge", trackNextPrestigeBadge);
		uxp_RewardsManager.ResetActiveRewards ();
		CloseBlurEffect ();
	}

	//Here we close our prestige popup.
	public void ClosePrestigePopup(){

		prestigePopup.SetActive (false);
		CloseBlurEffect ();
	}

	//Here we set our blur effect when showing reward popups.
	public void SetBlurEffect(){

		PostProcessingProfile profile = filters.profile;
		profile.depthOfField.enabled = true;
	}

	//Here we close our blur effect when showing reward popups.
	public void CloseBlurEffect(){

		PostProcessingProfile profile = filters.profile;
		profile.depthOfField.enabled = false;
	}

	//Here we update our target xp against our xp data file to see what rank we are
	public void UpdateTargetXP(){

		XpManager.GetRankIndex();
		targetXP = System.Int32.Parse (xpLadder[XpManager.rankIndex]);
		XpManager.CalculateTargetXP (targetXP);
	}

	// Here we update all our methods. Instead of filling our update with all the code we split it into methods for a nice cleaner work flow.
	void Update () {

		CheckPrestigeStatus ();
		UpdateTargetXP ();
		UpdateUIElements ();
		ManageRankMode ();
		ShowRequiredXP ();

	}

	//Here we check if the player can prestige or not.
	public void CheckPrestigeStatus(){

		int canPrestige = PlayerPrefs.GetInt ("CanPrestige");
		if (canPrestige == 1 && XpManager.usingPrestigeMode) {
			prestigeButton.SetActive (true);
		}
	}

	//Here we update all our ui elements with our player data.
	public void UpdateUIElements(){

		xpProgressBar.maxValue = XpManager.xpBarValue;
		curXPText.text = PlayerPrefs.GetInt ("CurXP", 0).ToString() + (" XP");
		xpProgressBar.value = PlayerPrefs.GetInt ("CurXP");
		curRankText.text = PlayerPrefs.GetString ("RankTitle");
		nextRankText.text = PlayerPrefs.GetString ("NextRankTitle");
		//curLvlText.text = ("LVL ") + PlayerPrefs.GetInt ("Rank").ToString();
		curLvlText.text = PlayerPrefs.GetInt ("Rank").ToString();
	}


	//Here we check to see if we are using prestige mode, If we are not we do not show the prestige mode option. This method also display's our current xp, cur badge and next rank.
	public void ManageRankMode(){


		XpManager.RankIconPath ();
		XpManager.GetRankIndex ();
		int prestigeLvl =  PlayerPrefs.GetInt ("CurPrestigeLvl");

		if (XpManager.inPrestigeMode == 1 && XpManager.usingRankBadges) {
			curRankBadgeLarge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + prestigeLvl);
			curRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + prestigeLvl);

			XpManager.RefeshRanks ();
			if (XpManager.curRank == maxLevel && prestigeLvl == xpOptions.SetMaxAllowedPrestiges) {
				nextRankBadgeObj.SetActive (false);
				prestigeButton.SetActive (false);
				PlayerPrefs.SetInt ("CanPrestige", 0);
			} 
			else 
			{
				if (XpManager.curRank == maxLevel) {
					trackNextPrestigeBadge = PlayerPrefs.GetInt ("NextPrestigeBadge");
					nextRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + trackNextPrestigeBadge);
				} else {
					nextRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + prestigeLvl);
				}
			}
		} 
		else if(XpManager.usingRankBadges) {
			curRankBadgeLarge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + XpManager.rankIndex);
			curRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + XpManager.rankIndex);
			nextRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + XpManager.nextRankIndex);
		}
	}

	//Here we check to see if we are displaying requiredXP and not targetXP.
	public void ShowRequiredXP(){

		if (!XpManager.showRequiredXP) {
			reqXPText.text = targetXP.ToString() +  (" XP");
		} else {
			reqXPText.text = PlayerPrefs.GetInt ("RequiredXP").ToString() +  (" XP");
		}
	}
}
