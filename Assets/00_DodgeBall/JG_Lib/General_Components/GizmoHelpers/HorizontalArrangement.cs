using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class HorizontalArrangement : MonoBehaviour
    {
        [SerializeField] GameObject piece = null;
        [SerializeField] int piecesCount = 4;
        [SerializeField] float totalDist = 4;
        [SerializeField] Vector3 baseLocalVec = Vector3.forward;
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

            float distBetweenPieces = totalDist / piecesCount;
            for (int i = 0; i < piecesCount; i++)
            {
                GameObject g = pieces[i];
                g.transform.localPosition = baseLocalVec * distBetweenPieces * i;
            }
        }
    }
}