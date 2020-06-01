using UnityEngine;

namespace GW_Lib.Utility
{
    public class GOActivitySetter : MonoBehaviour
    {
        [System.Serializable]
        class Activator
        {
            [SerializeField] GameObject go = null;
            [SerializeField] bool allowActivity = false;

            public Activator()
            {

            }
            public void CheckActivity()
            {
                if (allowActivity)
                {
                    return;
                }
                if (go == null)
                {
                    return;
                }
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }
        }

        [SerializeField] Activator[] activators = new Activator[1];

        void Update()
        {
            foreach (Activator a in activators)
            {
                a.CheckActivity();
            }
        }
    }
}