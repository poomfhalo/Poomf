using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class SpinArrangement : MonoBehaviour
    {
        [SerializeField] GameObject piece = null;
        [SerializeField] int piecesCount = 4;

        [SerializeField] Vector3 baseVAngle = new Vector3(0,0,90);
        [SerializeField] SpinAxis spinAxis = SpinAxis.Y;
        enum SpinAxis{X,Y,Z}

        [Header("Read Only")]
        [SerializeField] List<GameObject> pieces = new List<GameObject>();
        void Start()
        {
            Arrange();
        }
        void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Arrange();
        }
        private void Arrange()
        {
            if (piece == null)
            {
                return;
            }
            if (pieces.Count != piecesCount && piecesCount >= 0)
            {
                int remaining = piecesCount - pieces.Count;
                if (remaining > 0)
                {
                    for (int i = 0; i < remaining; i++)
                    {
                        GameObject g = Instantiate(piece, transform);
                        g.transform.localPosition = Vector3.zero;
                        pieces.Add(g);
                    }
                }
                else if (remaining < 0)
                {
                    remaining = Mathf.Abs(remaining);
                    for (int i = 0; i < remaining; i++)
                    {
                        GameObject piece = pieces[pieces.Count - 1];
                        pieces.Remove(piece);
                        if (Application.isPlaying)
                        {
                            Destroy(piece);
                        }
                        else
                        {
                            DestroyImmediate(piece);
                        }
                    }
                }
            }

            if (piecesCount <= 0)
            {
                return;
            }

            float angleBetweenPieces = 360 / piecesCount;
            for (int i = 0; i < piecesCount; i++)
            {
                GameObject g = pieces[i];
                float angle = angleBetweenPieces * i;
                Vector3 eu = baseVAngle;
                switch (spinAxis)
                {
                    case SpinAxis.X:
                        eu.x = angle;
                        break;
                    case SpinAxis.Y:
                        eu.y = angle;
                        break;
                    case SpinAxis.Z:
                        eu.z = angle;
                        break;
                }
                g.transform.localEulerAngles = eu;
            }
        }

    }
}