using System;
using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    [RequireComponent(typeof(UnitEventsCollection))]
    public abstract class UnitPlayable : MonoBehaviour
    {
        public event Action onCompleted;
        public int ID => id;
        [SerializeField] int id = 0;
        protected virtual void Reset()
        {
            SetID(GetComponents<UnitPlayable>().Length - 1);
        }
        public IEnumerator Play()
        {
            yield return StartCoroutine(Behavior());
            onCompleted?.Invoke();
        }
        public abstract IEnumerator Behavior();

        //called from editor
        public void SetID(int id)
        {
            this.id = id;
        }
    }
}