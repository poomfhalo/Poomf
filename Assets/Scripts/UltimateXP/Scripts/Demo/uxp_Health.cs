using UnityEngine;
using System.Collections;

public class uxp_Health : MonoBehaviour {

	uxp_XP xp;
	Renderer rend;
	public float mHealth = 1f;
	public float hitPoints = 100f;
	float currentHitPoints;

	void Start(){

		uxp_XPFeedManager.Initialize();
		xp = GetComponent<uxp_XP> ();
		rend = GetComponent<Renderer> ();
		currentHitPoints = hitPoints;
	}

	public void TakeDamage(float amt) {

		currentHitPoints -= amt;
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Play ();
		if (currentHitPoints <= 50) {

			rend.material.shader = Shader.Find("Standard");
			rend.material.SetColor("_Color", Color.red);
		}
		if(currentHitPoints <= 0) {
			Die();
		}
	}

	public void ReduceHealth(){

		mHealth -= 0.5f;

	}

	public void Die(){
		
		xp.XpReward ();
		uxp_XPFeedManager.CreateXPText(xp.xpAmount.ToString(), transform);
		DestroyAI ();
	}

	void DestroyAI(){
		
		mHealth = 1f;
		gameObject.SetActive (false);
		Destroy (gameObject);

	}
}
