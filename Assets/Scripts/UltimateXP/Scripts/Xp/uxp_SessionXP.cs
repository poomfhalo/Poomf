using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class uxp_SessionXP : MonoBehaviour {

	[Header("Rank Animation")]
	public Animator anim;
	public float rankAnimationTime = 3f;
	[Header("Rank Name")]
	public Text rankName;
	public Image newRankBadge;
	[Header("Rank Up Audio")]
	public AudioClip levelUpSound;
	AudioSource audioSource;

	[HideInInspector]
	public int xpAmount;
	[HideInInspector]
	public int xpMultiplier;
	private int targetXP;
	private int maxXp;
	private int xpCap;
	private TextAsset xpData;
	private TextAsset rankData;
	private string [] rankLadder;
	private string [] xpLadder;
	private int totalActualXP;


	void Start(){

		XpManager.RefeshRanks ();
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		LoadResourceData ();
	}

	//Here we load our xpData and rankData. We also update our maxXP once again incase the main game scene is loaded first
	public void LoadResourceData(){

		xpData = Resources.Load<TextAsset> ("XPData/XPData");
		rankData = Resources.Load<TextAsset> ("RankData/RankData");
		xpLadder= (xpData.text.Split( '\n' ));
		rankLadder= (rankData.text.Split( '\n' ));
		maxXp = xpLadder.Length - 1;
		xpCap = System.Int32.Parse (xpLadder [maxXp]);
		PlayerPrefs.SetInt ("MaxXpLevel", xpCap);
		totalActualXP =  PlayerPrefs.GetInt ("MaxTotalXP");
	}

	//Here we add our chosen xp amount to our XpManager.cs
	public void AddXP(){

		XpManager.xpCap = totalActualXP;
		XpManager.CalculateNewXP (xpAmount, xpMultiplier);
		XpManager.CalculateCareerXP (xpAmount, xpMultiplier);
		UpdateTargetXP ();
		CheckNewForRank ();
	}

	//Here we check if we have been promoted, If we have we run the new rank animation and update our rank title.
	public void CheckNewForRank(){
		
		if (XpManager.showNewRank == 1) {
			XpManager.GetRankIndex ();
			rankName.text = (rankLadder[XpManager.rankIndex]).ToString();
			XpManager.RankIconPath ();
			XpManager.GetRankIndex ();
			PrestigeMode ();
			RankUpBadges ();
			XpManager.showNewRank = 0;
		}
	}

	//Here we check to see if we are using prestige mode, If we are we update the rank badges.
	public void PrestigeMode(){

		int prestigeLvl =  PlayerPrefs.GetInt ("CurPrestigeLvl");

		if (XpManager.inPrestigeMode == 1) {
			newRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + prestigeLvl);
		} else {
			newRankBadge.sprite = Resources.Load<Sprite> (XpManager.rankIconPath + "/" + XpManager.rankIndex);
		}
	}

	//Here we check to see if we are using rank badges
	public void RankUpBadges(){

		if (XpManager.usingRankBadges) {
			string newRankTitle = rankName.text.ToString ();
			PlayerPrefs.SetString ("RankTitle", newRankTitle);
			string nextRankTitle = (rankLadder [XpManager.nextRankIndex]).ToString ();
			PlayerPrefs.SetString ("NextRankTitle", nextRankTitle);
			RankUpAnimation ();


		} else {
			string newRankTitle = rankName.text.ToString ();
			PlayerPrefs.SetString ("RankTitle", newRankTitle);
			string nextRankTitle = (rankLadder [XpManager.nextRankIndex]).ToString ();
			PlayerPrefs.SetString ("NextRankTitle", nextRankTitle);
		}
	}

	//Here we check to see if we are using the rank animation, If we are we play the anim.
	public void RankUpAnimation(){

		if (!XpManager.usingRankAnimations) {
			return;
		} else {
			anim.SetBool ("canMove", true);
			audioSource.PlayOneShot (levelUpSound, 0.7F);
			StartCoroutine (CloseNewRankPopup ());
		}
	}

	//Here we update our target xp against our xp data file to see what rank we are.
	public void UpdateTargetXP(){
		if (XpManager.curXP >= totalActualXP) {
			return;
		} else {
			XpManager.GetRankIndex ();
			targetXP = System.Int32.Parse (xpLadder [XpManager.rankIndex]);
			maxXp = xpLadder.Length - 1;
			xpCap = System.Int32.Parse (xpLadder [maxXp]);
			PlayerPrefs.SetInt ("MaxXpLevel", xpCap);
			XpManager.CalculateTargetXP (targetXP);
		}
    }

	//Here we control how long we show the new rank popup
	public IEnumerator CloseNewRankPopup(){

		yield return new WaitForSeconds (rankAnimationTime);
		anim.SetBool ("canMove", false);
		if (XpManager.showNewRank == 1) {

			CheckNewForRank ();
		}
	}

	//Here we make sure our target xp keeps upto date
	void Update(){
		
		UpdateTargetXP ();
	}
}
