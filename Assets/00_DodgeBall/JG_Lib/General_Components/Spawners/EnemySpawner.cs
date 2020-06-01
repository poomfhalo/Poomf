using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] float timeBeforeActivation = 1.0f;
        [SerializeField] GameObject spawnVFX = null;
        [SerializeField] bool waitForParticlesToFinish = false;
        public GameObject SpawnGameObject(GameObject obj,Transform spawnPoint)
        {
            GameObject enemy = Instantiate(obj);
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            obj.transform.SetParent(spawnPoint);

            enemy.SetActive(false);
            StartCoroutine(Activate(enemy));
            return enemy;
        }

        public GameObject EnableGameObject(GameObject obj, Transform spawnPoint)
        {
            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = spawnPoint.rotation;
            StartCoroutine(Activate(obj));
            return obj;
        }

        private IEnumerator Activate(GameObject enemy)
        {
            if(spawnVFX == null)
            {
                enemy.gameObject.SetActive(true);
                yield break;
            }

            GameObject vfx = Instantiate(spawnVFX);
            vfx.transform.position = enemy.transform.position;
            vfx.transform.rotation = enemy.transform.rotation;
            yield return new WaitForSeconds(timeBeforeActivation);
            if(waitForParticlesToFinish)
            {
                ParticleSystem ps = vfx.GetComponentInChildren<ParticleSystem>();
                while(ps && ps.isPlaying)
                {
                    yield return 0;
                }
            }
            enemy.gameObject.SetActive(true);
        }
    }
}