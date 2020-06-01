using UnityEngine;

namespace GW_Lib.Utility
{
    public class SwitchObjActivity : MonoBehaviour
    {
        [SerializeField] GameObject objToSwitch = null;
        [SerializeField] float timeToSwitch = 0.01f;

        float counter = 0;
        bool isSwitching = false;

        public void DoSwitch()
        {
            isSwitching = true;
            counter = 0;
            objToSwitch.SetActive(!objToSwitch.activeSelf);
        }
        void Update()
        {
            if(!isSwitching)
            {
                return;
            }
            counter = counter + Time.deltaTime/timeToSwitch;
            if(counter<1)
            {
                return;
            }

            isSwitching = false;
            objToSwitch.SetActive(!objToSwitch.activeSelf);
        }
    }
}