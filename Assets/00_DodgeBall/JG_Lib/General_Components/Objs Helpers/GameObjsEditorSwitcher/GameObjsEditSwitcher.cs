using UnityEngine;

namespace GW_Lib.Utility
{
    public class GameObjsEditSwitcher : MonoBehaviour
    {
        public GameObject[] objs = new GameObject[0];

        private void Awake()
        {
            if(Application.isPlaying)
            {
                Destroy(this);
            }
        }
        public void ActivateObj(int x)
        {
            if(objs.Length == 0)
            {
                return;
            }
            if(x<0||x>=objs.Length)
            {
                 x = Mathf.Clamp(x,0,objs.Length-1);
            }
            foreach(GameObject g in objs)
            {
                if(g==null) { continue; }

                if(g.activeSelf)
                {
                    g.SetActive(false);
                }
            }
            if(objs[x] == null) { return; }
            objs[x].SetActive(true);
        }

    }
}