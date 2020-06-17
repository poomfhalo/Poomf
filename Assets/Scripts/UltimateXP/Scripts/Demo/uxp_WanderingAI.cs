using UnityEngine;
using System.Collections;

public class uxp_WanderingAI : MonoBehaviour {
	
	public float wanderRadius;
	public float wanderTimer;
	public bool isAlive = false;
	private UnityEngine.AI.NavMeshAgent agent;
	private float timer;

	// Use this for initialization
	void OnEnable () {
		
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		timer = wanderTimer;
	}

	// Update is called once per frame
	void Update () {

			if (isAlive) {
				timer += Time.deltaTime;

				if (timer >= wanderTimer) {
					Vector3 newPos = RandomNavSphere (transform.position, wanderRadius, -1);
					agent.SetDestination (newPos);
					agent.speed = 3.5f;
					timer = 0;
			}
		} 
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		UnityEngine.AI.NavMeshHit navHit;

		UnityEngine.AI.NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;
	}
}