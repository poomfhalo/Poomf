using UnityEngine;
using UnityEngine.UI;

namespace StealthGame.Utility
{
    public class FPSDisplayer : MonoBehaviour
    {
        [SerializeField] Text simpleFrameRateText = null;
        [Header("Complex Frame Rate")]
        [SerializeField] Text complexFrameRateText = null;
        [SerializeField] float period = 1;
        [Header("Pause Settings")]
        [SerializeField] bool pauseOnLowFPS = false;
        [SerializeField] float lowFPS = 55;

        float counter;
        int framesCount;
        float complexFrameRate;
        float simpleFrameRate;

        void Update()
        {
            simpleFrameRate = 1.0f / Time.deltaTime;
            simpleFrameRateText.text = "Simple FPS: " + simpleFrameRate;
            ComplexFrameRate();
            if (pauseOnLowFPS)
            {
                if (simpleFrameRate < lowFPS && complexFrameRate < lowFPS)
                {
                    Debug.Break();
                    Debug.Log("Paused Due To Low FPS");
                }
            }
            
        }

        private void ComplexFrameRate()
        {
            if (counter<period)
            {
                counter += Time.deltaTime;
                framesCount++;
            }
            else
            {
                complexFrameRate = framesCount/counter;
                framesCount = 0;
                counter = 0;
                complexFrameRateText.text = "Frame Rate Over Period Of_" + period + "_is " + complexFrameRate;
            }
        }
    }
}