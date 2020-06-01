using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class TriggerDelegatorsGroup : MonoBehaviour
    {
        [SerializeField] string[] tags = new string[0];
        [SerializeField] TriggerDelegator[] delegators = new TriggerDelegator[0];

        [Header("Read Only")]
        [SerializeField] List<Collider> inCols = new List<Collider>();

        protected virtual void Awake()
        {
            if(delegators.Length == 0)
                delegators = GetComponentsInChildren<TriggerDelegator>();
            foreach (TriggerDelegator delegator in delegators)
            {
                delegator.onTriggerEnter.AddListener(OnEntered);
                delegator.onTriggerExit.AddListener(OnExit);
            }
        }
        public bool isFull => inCols.Count != 0;
        public GameObject GetCol(int v)
        {
            GameObject g = null;
            if(inCols.Count == 0)
            {
                g = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                v = Mathf.Clamp(v, 0, inCols.Count - 1);
                g = inCols[v].gameObject;
            }

            return g;
        }

        private void OnEntered(Collider col)
        {
            if (!IsGoodCol(col))
            {
                return;
            }
            if(inCols.Contains(col))
            {
                return;
            }
            inCols.Add(col);
        }
        private void OnExit(Collider col)
        {
            if (!IsGoodCol(col))
            {
                return;
            }
            if(!inCols.Contains(col))
            {
                return;
            }
            inCols.Remove(col);
        }
        private bool IsGoodCol(Collider col)
        {
            foreach (string s in tags)
            {
                if (col.CompareTag(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}