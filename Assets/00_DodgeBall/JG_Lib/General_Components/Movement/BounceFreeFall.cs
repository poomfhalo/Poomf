using UnityEngine;

namespace GW_Lib.Utility
{
    public class BounceFreeFall : MonoBehaviour
    {
        public ForceMode forceMode;
        public float forceMag = 10;
        Rigidbody rb3d;
        Vector3 lastPos = new Vector3();
        public LayerMask mask;
        void Start()
        {
            rb3d = GetComponent<Rigidbody>();
            lastPos = transform.position;
        }
        void Update()
        {
            Vector3 dir = transform.position - lastPos;
            float maxDist = dir.magnitude + transform.localScale.x / 2.0f;
            Ray ray = new Ray(transform.position, dir.normalized);
            RaycastHit whatWasHit;
            Debug.DrawRay(transform.position, dir.normalized * maxDist);
            bool didHit = Physics.Raycast(ray, out whatWasHit, maxDist, mask);
            if (didHit)
            {
                rb3d.velocity = Vector3.zero;
                Vector3 f = new Vector3();
                f = Vector3.Reflect(ray.direction, whatWasHit.normal) * forceMag;
                rb3d.AddForce(f, forceMode);
            }
            lastPos = transform.position;
        }
    }
}