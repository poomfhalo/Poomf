using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class YDisplacerArrangement : MonoBehaviour
    {
        public Transform[] Arrangeables => arrangeables;
        [Tooltip("Usable If Arrange Children Is False")]
        [SerializeField] Transform[] arrangeables = new Transform[0];
        [SerializeField] bool arrangeChildren = true;
        [SerializeField] MinMaxRange rndHeigth = new MinMaxRange(-5, 5, -3, 3);

        [Header("Editor Properties")]
        [SerializeField] bool moveInEditor = false;
        [SerializeField] bool drawGizmoOnEachPoint = false;
        [SerializeField] float pointRadious = 0.15f;

        [Header("Read Only")]
        [SerializeField] MinMaxRange lastRndHeigth = new MinMaxRange();

        void Start()
        {
            if (moveInEditor && !Application.isPlaying)
                Arrange();
            if (Application.isPlaying)
                Arrange();
        }

        void Update()
        {
            if (moveInEditor && !Application.isPlaying)
            {
                Arrange();
            }
            else if (Application.isPlaying)
            {
                Arrange();
            }
        }

        private void Arrange()
        {
            if (lastRndHeigth.Equals(rndHeigth))
                return;

            if (arrangeChildren)
                SetArrangablesAsChildren();

            if (arrangeables.Length == 0)
                return;

            for (int i = 0; i < arrangeables.Length; i++)
            {
                Transform a = arrangeables[i];
                float y = rndHeigth.GetValue();
                Vector3 pos = a.localPosition;
                pos.y = y;
                a.localPosition = pos;
            }
            lastRndHeigth = new MinMaxRange(rndHeigth);
        }
        public void SetArrangablesAsChildren()
        {
            arrangeables = new Transform[transform.childCount];
            for (int i = 0; i < arrangeables.Length; i++)
            {
                arrangeables[i] = transform.GetChild(i);
            }
        }
        void OnDrawGizmos()
        {
            if (drawGizmoOnEachPoint == false)
            {
                return;
            }
            foreach (Transform t in arrangeables)
            {
                Gizmos.DrawWireSphere(t.position, pointRadious);
            }
        }

    }
}