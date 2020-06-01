using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class ArcArrangement : MonoBehaviour
    {
        public bool MoveInEditor => moveInEditor;
        public bool ArrangeChildren => arrangeChildren;
        public float Radious => radious;
        public float Angle => angle;
        public Transform[] Arrangeables => arrangeables;

        [SerializeField] float angle = 360.0f;
        [SerializeField] float radious = 1.25f;
        [SerializeField] bool arrangeChildren = true;
        [Tooltip("Usable If Arrange Children Is False")]
        [SerializeField] Transform[] arrangeables = new Transform[0];
        [SerializeField] bool pointsFaceCenter = true;
        [SerializeField] float lerpSpeed = 10;

        [Header("Editor Properties")]
        [SerializeField] bool drawGizmoOnEachPoint = false;
        [SerializeField] float pointRadious = 0.15f;
        [SerializeField] bool moveInEditor = true;

        void Start()
        {
            Arrange();
        }
        void Update()
        {
            if (moveInEditor && !Application.isPlaying)
            {
                Arrange();
            }
            else if(Application.isPlaying)
            {
                Arrange();
            }
        }

        public void Arrange()
        {
            if (arrangeChildren)
            {
                SetArrangablesAsChildren();
            }

            if (arrangeables.Length == 0)
            {
                return;
            }

            float degreeStep = angle / arrangeables.Length;

            for (int i = 0; i < arrangeables.Length; i++)
            {
                Transform t = arrangeables[i];

                float degree = degreeStep * i;
                Vector3 rotatedForward = Quaternion.Euler(0, degree, 0) * transform.forward;
                float dt = Time.deltaTime;
                Vector3 newPos = rotatedForward * radious;
                newPos.y = t.localPosition.y;
                t.localPosition = Vector3.Lerp(t.localPosition, newPos, lerpSpeed * dt);
                if (pointsFaceCenter)
                {
                    t.transform.localRotation = Quaternion.LookRotation(-t.localPosition);
                }
            }
        }

        public void SetArrangablesAsChildren()
        {
            arrangeables = new Transform[transform.childCount];
            for (int i = 0; i < arrangeables.Length; i++)
            {
                arrangeables[i] = transform.GetChild(i);
            }
        }
        private void OnDrawGizmos()
        {
            if (drawGizmoOnEachPoint == false)
            {
                return;
            }
            Gizmos.color = Color.cyan;
            foreach (Transform t in arrangeables)
            {
                Gizmos.DrawWireSphere(t.position, pointRadious);
            }
        }
    }
}