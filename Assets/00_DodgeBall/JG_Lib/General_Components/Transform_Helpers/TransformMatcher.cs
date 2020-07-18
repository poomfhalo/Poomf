using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class TransformMatcher : MonoBehaviour
    {
        [SerializeField] Transform xRot = null;
        [SerializeField] Transform yRot = null;
        [SerializeField] Transform pos = null;
        [SerializeField] Vector3 posOffset = Vector3.zero;
        [SerializeField] bool runInEditor = false;

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if(runInEditor)
                    DoMatch();

                return;
            }

            DoMatch();
        }

        public void DoMatch()
        {
            if (xRot && yRot)
            {
                Vector3 eu = new Vector3(xRot.transform.eulerAngles.x, yRot.rotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Euler(eu);
            }
            if (pos)
            {
                transform.position = pos.position + posOffset;
            }
        }
    }
}