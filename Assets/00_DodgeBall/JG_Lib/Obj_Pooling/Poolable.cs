using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility
{
    public class Poolable : MonoBehaviour
    {
        public int prefabID => prefabIDHash;
        public int instID => gameObject.GetInstanceID();
        public bool isActive { private set; get; }
        public event Action<bool> OnSetActive;

        [Tooltip("if true, then the SetActive activity will affect GO activity")]
        [SerializeField] bool disableGO = true;

        [Header("Set From PoolsManager Inspector")]
        [Tooltip("Go to PoolsManager inspector in hirearchy then click Update GUIDS button to get new UUID")]
        [SerializeField] int prefabIDHash = 0;
        [SerializeField] UnityEvent onActivated = null;

        public void SetActive(bool activity)
        {
            isActive = activity;
            if (disableGO)
            {
                gameObject.SetActive(activity);
            }
            OnSetActive?.Invoke(activity);
            if(activity)
            {
                onActivated?.Invoke();
            }
        }
        public void FreeSelf(int delay)
        {
            if (delay <= 0)
            {
                PoolsManager.instance.FreeGO(gameObject);
            }
            else
            {
                StartCoroutine(DoFreeSelf(delay));
            }
        }

        private IEnumerator DoFreeSelf(float delay)
        {
            for (int i = 0; i < delay; i++)
            {
                yield return 0;
            }
            PoolsManager.instance.FreeGO(gameObject);
        }

        //Called from the editor to set the new Prefab ID
        public void SetPrefabID(int hash)
        {
            prefabIDHash = hash;
        }
    }
}