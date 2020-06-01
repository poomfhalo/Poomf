using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    [RequireComponent(typeof(Poolable))]
    public class PoolableEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem ps = null;
        [SerializeField] GameObject sfx = null;
        [SerializeField] int freeTime = 3;

        Poolable poolable;

        void Awake()
        {
            poolable = GetComponent<Poolable>();
            poolable.OnSetActive += OnSetActive;
        }

        private void OnSetActive(bool activity)
        {
            if (activity)
            {
                StartCoroutine(AutoDisable());
            }
            if (activity)
            {
                if (ps)
                {
                    ps.Play(true);
                }
            }
            else
            {
                if (ps)
                {
                    ps.Stop(true);
                }
            }
            if (sfx)
            {
                sfx.SetActive(activity);
            }
        }

        private IEnumerator AutoDisable()
        {
            yield return new WaitForSeconds(freeTime);
            PoolsManager.instance.FreeGO(gameObject);
        }
    }
}