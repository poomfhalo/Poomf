using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SpawnRandomObj : UnitPlayable
    {
        enum SpawnMode
        {
            Random, Unique
        }
        [SerializeField] SpawnMode spawnMode = SpawnMode.Random;
        [SerializeField] GameObject[] objsToSpawn = new GameObject[0];

        [Header("Only Used In Unique Spawn Mode")]
        [SerializeField] int uniqueIndex = 0;
        
        public override IEnumerator Behavior()
        {
            if (objsToSpawn.Length == 0)
            {
                yield break;
            }

            GameObject o = null;
            switch (spawnMode)
            {
                case SpawnMode.Random:
                    int randIndex = Random.Range(0, objsToSpawn.Length - 1);
                    o = objsToSpawn[randIndex];
                    break;
                case SpawnMode.Unique:
                    int index = Mathf.Clamp(uniqueIndex, 0, objsToSpawn.Length - 1);
                    o = objsToSpawn[index];
                    break;
                default:
                    Debug.LogWarning("UnDefined Spawn Mode");
                    break;
            }
            if (o)
            {
                SpawnObj(o);
            }
        }

        private void SpawnObj(GameObject o)
        {
            GameObject spawnedObj = null;

            if (o.GetComponent<Poolable>())
            {
                spawnedObj = PoolsManager.instance.GetPoolable(o.GetComponent<Poolable>(), transform.position);
            }
            else
            {
                spawnedObj = Instantiate(o);
            }

            spawnedObj.transform.position = transform.position;
            spawnedObj.transform.rotation = transform.rotation;
        }
    }
}