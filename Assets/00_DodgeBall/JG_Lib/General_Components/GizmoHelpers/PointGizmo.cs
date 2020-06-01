using UnityEngine;

namespace GW_Lib.Utility
{
    public class PointGizmo : MonoBehaviour
    {
        [SerializeField] Color color = Color.cyan;
        [SerializeField] float radious = 0.5f;
        [SerializeField] bool alwaysDraw = false;
        [SerializeField] bool neverDraw = false;

        void OnDrawGizmos()
        {
            if (alwaysDraw)
            {
                Draw();
            }
        }
        void OnDrawGizmosSelected()
        {
            Draw();
        }
        private void Draw()
        {
            if(neverDraw)
            {
                return;
            }
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position,radious);
        }
        public void SetUp(Color c,float radious)
        {
            this.radious = radious;
            this.color = c;
        }
    }
}