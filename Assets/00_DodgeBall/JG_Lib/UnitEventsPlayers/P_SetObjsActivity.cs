using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SetObjsActivity : UnitPlayable
    {
        [SerializeField] GameObject[] objs = new GameObject[0];
        [SerializeField] bool activity = false;
        
        public override IEnumerator Behavior()
        {
            foreach (GameObject g in objs)
            {
                g.SetActive(activity);
            }
            yield break;
        }
    }
}