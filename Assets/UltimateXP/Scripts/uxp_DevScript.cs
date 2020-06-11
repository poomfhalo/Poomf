using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//A simple script to reset stats and toggle max rank for testing
public class uxp_DevScript : MonoBehaviour {

	public GameObject prestigeButton;
	private TextAsset rankData;
	private string [] rankLadder;

	public void ResetStats () {

		XpManager.IntializeXPData ();
		prestigeButton.SetActive (false);
	}

	public void StartDemo(){

		SceneManager.LoadScene (1);

	}

	public void ExitGame(){

		Application.Quit ();
	}

	public void MaxStats(){

		PlayerPrefs.SetInt ("CurXP", PlayerPrefs.GetInt("MaxXpLevel"));
		PlayerPrefs.SetInt ("Rank", PlayerPrefs.GetInt("MaxLevel"));
		PlayerPrefs.SetInt ("NextRank", PlayerPrefs.GetInt("MaxTotalLevel"));
		rankData = Resources.Load<TextAsset> ("RankData/RankData");
		rankLadder= (rankData.text.Split( '\n' ));
		int maxRankTitle = rankLadder.Length -2;
		PlayerPrefs.SetString ("RankTitle", rankLadder[maxRankTitle].ToString());
		prestigeButton.SetActive (true);
	}
}
