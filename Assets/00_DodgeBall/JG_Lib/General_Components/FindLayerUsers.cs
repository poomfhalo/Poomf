using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GW_Lib.Utility
{
    public class FindLayerUsers : MonoBehaviour
    {
        public string lName = "LightsSelection";
        void Start()
        {
            var tes = GW_Lib.Extentions.GetAllObjectsInScene<Transform>(SceneManager.GetActiveScene());
            foreach (var t in tes)
            {
                if (LayerMask.LayerToName(t.gameObject.layer) == lName)
                {
                    Debug.Log(t.name, t.gameObject);
                }
            }

        }
    }
}