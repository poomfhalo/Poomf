using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class Pool
    {
        HashSet<int> inUse = new HashSet<int>();
        List<int> unused = new List<int>();

        Dictionary<int, Poolable> population = new Dictionary<int, Poolable>();
        Poolable prefab;
        Transform poolablesParent;

        public Pool(Poolable prefab, int startCount, Transform poolParent)
        {
            this.prefab = prefab;

            poolablesParent = new GameObject("Pool (" + prefab.name + ")").transform;
            poolablesParent.SetParent(poolParent);

            Populate(startCount);
        }

        public void Populate(int count)
        {
            inUse = new HashSet<int>();
            unused = new List<int>();
            population = new Dictionary<int, Poolable>();

            for (int i = 0; i < count; i++)
            {
                CreateElement();
            }
        }
        public Poolable GetElement()
        {
            Poolable element = null;
            if (unused.Count <= 0)
            {
                CreateElement();
            }
            int id = unused[0];

            unused.Remove(id);
            inUse.Add(id);

            element = population[id];
            element.SetActive(false);
            return element;
        }
        public bool ReleaseElement(int id)
        {
            if (inUse.Contains(id) == false)
            {
                return false;
            }
            inUse.Remove(id);
            unused.Add(id);

            population[id].SetActive(false);
            return true;
        }

        private void CreateElement()
        {
            Poolable poolable = GameObject.Instantiate(prefab, poolablesParent);
            poolable.SetActive(false);
            population.Add(poolable.instID, poolable);
            unused.Add(poolable.instID);
        }
    }
    [System.Serializable]
    public class PoolElements
    {
        public Poolable prefab = null;
        public int count = 50;

        public PoolElements(Poolable prefab, int count)
        {
            this.prefab = prefab;
            this.count = count;
        }
        public PoolElements()
        {
            
        }
    }
    public class PoolsManager : Singleton<PoolsManager>
    {
        [SerializeField] int defPoolingCount = 50;
        [SerializeField] List<PoolElements> startingPools = new List<PoolElements>(1);

        [Header("Editor Script Helpers")]
        public string prefabsPath = "Assets/00_Game";

        Dictionary<int, Pool> pools = new Dictionary<int, Pool>();

        void Start()
        {
            foreach (PoolElements p in startingPools)
            {
                CreatePool(p);
            }
        }
        public void CreatePool(PoolElements p)
        {
            int id = p.prefab.prefabID;
            int count = p.count;
            if(p.count == 0)
            {
                Debug.Log(p.prefab.name + " is trying to be pooled with zero "+
                                            "instances, will use default count instead");
                count = defPoolingCount;
            }
            pools[id] = new Pool(p.prefab, count, transform);
        }
        public GameObject GetPoolable(Poolable prefab, Vector3 atPos)
        {
            if(prefab == null)
            {
                return null;
            }

            int id = prefab.prefabID;
            if (pools.ContainsKey(id) == false)
            {
                pools[id] = new Pool(prefab, defPoolingCount, transform);
            }

            Poolable element = pools[id].GetElement();
            element.transform.position = atPos;
            element.SetActive(true);

            return element.gameObject;
        }
        public T GetPoolable<T>(GameObject prefab, Vector3 position)
        {
            GameObject g = GetPoolable(prefab.GetComponent<Poolable>(), position);
            if(g==null)
            {
                Debug.LogWarning("can not find poolable script on " + prefab.name,prefab);
                return default(T);
            }
            return g.GetComponent<T>();
        }
        public T GetPoolable<T>(T prefab,Vector3 atPos) where T: Component
        {
            return GetPoolable<T>(prefab.gameObject,atPos);
        }
        public GameObject GetPoolableG(GameObject prefab,Vector3 atPos)
        {
            return GetPoolable(prefab.GetComponent<Poolable>(),atPos);
        }
        public bool FreeGO(GameObject inst)
        {
            Poolable poolable = inst.GetComponent<Poolable>();
            if (poolable == null)
            {
                return false;
            }

            int id = poolable.prefabID;
            if (pools.ContainsKey(id) == false)
            {
                return false;
            }

            Pool p = pools[id];
            return p.ReleaseElement(poolable.instID);
        }

        public Coroutine SafeMakeObject(GameObject prefab,Transform spawnPoint,float delay,bool isSpawnPointParent,Action<GameObject> onCreated = null)
        {
            Coroutine c = StartCoroutine(MakeObject(prefab,spawnPoint,delay,isSpawnPointParent,onCreated));
            return c;
        }
        private IEnumerator MakeObject(GameObject prefab, Transform spawnPoint, float delay, bool isSpawnPointParent,Action<GameObject> onCreated)
        {
            yield return new WaitForSeconds(delay);
            GameObject inst = Instantiate(prefab);

            if (spawnPoint)
            {
                if (isSpawnPointParent)
                {
                    inst.transform.SetParent(spawnPoint);
                    inst.transform.localPosition = Vector3.zero;
                    inst.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    inst.transform.localPosition = spawnPoint.transform.position;
                    inst.transform.localRotation = spawnPoint.transform.rotation;
                }
            }
            else
            {
                inst.transform.position = transform.position;
                inst.transform.rotation = transform.rotation;
            }

            onCreated?.Invoke(inst);
        }
        //Editor Function used to add poolable to startingPools
        public bool TryAddToStartingPools(Poolable p)
        {
            foreach(PoolElements element in startingPools)
            {
                if(element.prefab == p)
                {
                    return false;
                }
            }
            startingPools.Add(new PoolElements(p,defPoolingCount));
            return true;
        }
    }
}