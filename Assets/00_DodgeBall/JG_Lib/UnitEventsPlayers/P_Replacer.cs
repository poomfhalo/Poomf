using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_Replacer : UnitPlayable
    {
        [SerializeField] GameObject objToReplace = null;
        [SerializeField] GameObject replacementPrefab = null;
        [SerializeField] float timeBeforeReplacing = 0.01f;
        [SerializeField] bool copyPos = true, copyRot = false;

        public override IEnumerator Behavior()
        {
            Replace();
            yield break;
        }
        private void Replace()
        {
            if (replacementPrefab != null)
            {
                CreateReplacement();
            }
            if (objToReplace)
            {
                Destroy(objToReplace, timeBeforeReplacing);
            }
        }

        private void CreateReplacement()
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            Transform t = objToReplace ? objToReplace.transform : transform;
            if (copyPos)
            {
                pos = t.position;
            }
            if (copyRot)
            {
                rot = t.rotation;
            }

            GameObject replacement = null;
            if (replacementPrefab.GetComponent<Poolable>())
            {
                replacement =
                     PoolsManager.instance.GetPoolable
                            (replacementPrefab.GetComponent<Poolable>(), pos);
            }
            else
            {
                replacement = Instantiate(replacementPrefab, objToReplace.transform.parent);
            }
            replacement.transform.position = pos;
            replacement.transform.rotation = rot;
        }
    }
}