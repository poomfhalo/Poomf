using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class uxp_DemoManager : MonoBehaviour {

	public void ExitDemoGame(){

		SceneManager.LoadScene (0);
	}
}
