using UnityEngine;

namespace GW_Lib.Utility
{
    public class LevelBoundGizmo : MonoBehaviour
    {
        [SerializeField] bool drawGizmo = true;
        [SerializeField] Color boundColor = Color.grey;
        private void OnDrawGizmos()
        {
            if (drawGizmo==false)
            {
                return;
            }
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
    }
}