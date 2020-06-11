using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uxp_AISpawner : MonoBehaviour {

	public List<GameObject> ai_Prefabs;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public GameObject[] spawnPoints;
	private GameObject currentPoint;
	private int index;

	void Start ()
	{
		StartCoroutine (SpawnWaves ());
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
				spawnPoints = GameObject.FindGameObjectsWithTag("Spawner");
				index = Random.Range(0, spawnPoints.Length);
				currentPoint = spawnPoints[index];
				GameObject ai = ai_Prefabs [Random.Range (0, ai_Prefabs.Count)];
				Instantiate (ai, currentPoint.transform.position, currentPoint.transform.rotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}
}