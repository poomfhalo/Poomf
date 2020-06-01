using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SetRecievedPos : UnitPlayable, IPositionReciever
    {
        [SerializeField] Transform transformToSet = null;

        Vector3 recievedPosition = Vector3.zero;
        public override IEnumerator Behavior()
        {
            if (transformToSet)
            {
                transformToSet.position = recievedPosition;
            }
            yield break;
        }

        public void RecievePosition(Vector3 position)
        {
            recievedPosition = position;
        }
    }
}