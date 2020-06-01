using UnityEngine;

namespace GW_Lib.Utility
{
    public class RegeneratablePoint : MonoBehaviour
    {
        [Header("Set in inspector to count as game start point")]
        [SerializeField] GameObject point = null;
        public bool hasPoint {get{return point != null;}}
        private void Reset()
        {
            if(transform.GetChild(0))
            {
                point = transform.GetChild(0).gameObject;
            }
        }
        public void Add(GameObject pointPrefab)
        {
            GameObject p = Instantiate(pointPrefab);
            this.point = p;
            p.transform.SetParent(transform);
            p.transform.localPosition = Vector3.zero;
            p.transform.localRotation = Quaternion.identity;
        }
        public void Remove()
        {
            if (point == null)
            {
                return;
            }
            Destroy(point);
        }
    }
}