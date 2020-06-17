using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class uxp_RewardEditor : EditorWindow {

	bool groupEnabled;
	public List<string> rewardActiveList = new List<string>();
	string test = "";
	string temp = "";
	string chosenIndex = "";

	[MenuItem("Window/UltimateXP/Reward Editor")]
	static void Init()
	{
		uxp_RewardEditor window = (uxp_RewardEditor)EditorWindow.GetWindow(typeof(uxp_RewardEditor));
		window.Show();
		window.minSize = new Vector2 (800, 600);
		window.maxSize = new Vector2 (800, 610);
	}

	void OnGUI()
	{

		GUILayout.Label("Manage Rewards", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal(GUI.skin.box);
		{
			GUILayout.BeginVertical (GUI.skin.box);
			GUI.backgroundColor = Color.magenta;
			if (GUILayout.Button ("Load Active Rewards", GUILayout.Width(313), GUILayout.Height (30))) 
			{
				ListAllActiveRewards ();
			}
			if (GUILayout.Button ("Reset All Active Rewards", GUILayout.Width (313), GUILayout.Height (30))) {
				foreach (string activeReward in rewardActiveList) {

					PlayerPrefs.SetInt (activeReward, 0);
					EditorSceneManager.SaveOpenScenes ();

				}
			}
				
			if (GUILayout.Button("Delete Reward", GUILayout.Height (30)))
			{
				int index =  System.Int32.Parse(chosenIndex);
				uxp_RewardsManager rewardsManager = FindObjectOfType <uxp_RewardsManager> ();
				rewardsManager.RemoveReward (index);
				test = "";
				temp = "";
				ListAllActiveRewards ();
				EditorSceneManager.SaveOpenScenes ();
			
		}
			chosenIndex = EditorGUILayout.TextField ("Index", chosenIndex);
			GUILayout.EndVertical();
			//Save box's...
			GUI.backgroundColor = Color.white;
			GUILayout.BeginVertical();
			{
				EditorGUILayout.TextArea(test, GUILayout.Width(320), GUILayout.MinHeight(position.height - 100));
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			{
				EditorGUILayout.TextArea(temp, GUILayout.Width(50), GUILayout.MinHeight(position.height - 100));

			}
			GUILayout.EndVertical ();
		}
	}

	void ListAllActiveRewards(){

		uxp_RewardsManager rewardsManager = FindObjectOfType <uxp_RewardsManager> ();
		rewardActiveList = rewardsManager.rewardNameList;
		test = "";
		temp = "";
		foreach (string msg in rewardActiveList) {
			test = test.ToString () + msg.ToString () + "\n";
		}
		for (int i = 0; i < rewardActiveList.Count; i++) {

			temp = temp.ToString() + i.ToString() + "\n";
		}
	}
}