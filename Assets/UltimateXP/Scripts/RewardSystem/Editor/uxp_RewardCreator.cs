using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

//THIS CLASS CREATES REWARDS AND ADDS THEM TO THE REWARD MANAGER
public class uxp_RewardCreator : EditorWindow {

	string rewardName = "";
	string rewardDescription = "";
	bool groupEnabled;
	string rewardValue;
	string rewardLevel;
	public Texture2D logo;

	[MenuItem("Window/UltimateXP/Reward Creator")]
	static void Init()
	{
		uxp_RewardCreator window = (uxp_RewardCreator)EditorWindow.GetWindow(typeof(uxp_RewardCreator));
		window.Show();
		window.minSize = new Vector2 (350,500);
		window.maxSize = new Vector2 (350,510);
	}

	void OnGUI()
	{
		GUI.backgroundColor = Color.black;
		GUILayout.Label("Reward", EditorStyles.boldLabel);
		GUI.backgroundColor = Color.grey;
		rewardName = EditorGUILayout.TextField("Reward Name", rewardName, GUILayout.Width(320));
		GUI.backgroundColor = Color.black;
		GUILayout.Label("Reward Info", EditorStyles.boldLabel);
		GUI.backgroundColor = Color.grey;
		rewardDescription = EditorGUILayout.TextArea(rewardDescription, GUILayout.Width(320), GUILayout.MinHeight(position.height - 450));
		GUI.backgroundColor = Color.black;
		GUILayout.Label("Reward Rank", EditorStyles.boldLabel);
		GUI.backgroundColor = Color.grey;
		rewardLevel = EditorGUILayout.TextField ("Rank Required", rewardLevel, GUILayout.Width(320));
		GUI.backgroundColor = Color.black;
		GUILayout.Label("Reward Amount", EditorStyles.boldLabel);
		GUI.backgroundColor = Color.grey;
		rewardValue = EditorGUILayout.TextField ("Reward Value", rewardValue, GUILayout.Width(320));
		GUI.backgroundColor = Color.white;
		if(GUILayout.Button("Add Reward", GUILayout.Height(50)))
		{
			CreateNewReward ();
		}
		GUI.DrawTexture (new Rect (10, 330, 350, 150), logo, ScaleMode.StretchToFill, true, 10.0f);
	}
	void CreateNewReward(){

		uxp_RewardsManager rewardsManager = FindObjectOfType <uxp_RewardsManager> ();
		rewardsManager.AddReward  (rewardName, rewardDescription, System.Int32.Parse(rewardLevel), System.Int32.Parse(rewardValue));
		rewardName = "";
		rewardDescription = "";
		rewardLevel = "";
		rewardValue = "";
		EditorSceneManager.SaveOpenScenes ();
	}
}