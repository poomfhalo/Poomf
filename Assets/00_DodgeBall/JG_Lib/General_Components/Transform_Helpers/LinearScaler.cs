using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways()]
    public class LinearScaler : MonoBehaviour
    {
        [SerializeField] float minScale = 0.3f;
        [SerializeField] float maxScale = 10.0f;
        [SerializeField] float a = 1;

        [Header("Scale Limiters")]
        [SerializeField] bool scaleOnX = false;
        [SerializeField] bool scaleOnY = true;
        [SerializeField] bool scaleOnZ = false;

        [Header("In Editor Settings")]
        [SerializeField] float testX = 1;
        [SerializeField] Vector3 testStartScale = Vector3.one;
        [SerializeField] bool workInEditor = true;

        private Vector3 startScale;
        void Start()
        {
            startScale = transform.localScale;
        }
        void Update()
        {
            if(Application.isPlaying && workInEditor)
            {
                return;
            }
            Scale(testX);
        }
        public void Scale(float x)
        {
            Vector3 startScale = GetStartScale();
            Vector3 scale = startScale;
            if(scaleOnX)
            {
                scale.x = GetScaled(startScale.x,x);
            }
            if(scaleOnY)
            {
                scale.y = GetScaled(startScale.y,x);
            }
            if(scaleOnZ)
            {
                scale.z = GetScaled(startScale.z,x);
            }
            transform.localScale = scale;
        }
        private float GetScaled(float startVal,float x)
        {
            float scaledVal = 0;
            scaledVal = startVal*a*x;
            scaledVal = Mathf.Clamp(scaledVal,minScale,maxScale);
            return scaledVal;
        }

        private Vector3 GetStartScale()
        {
            if(Application.isPlaying)
            {
                return startScale;
            }
            return testStartScale;
        }
    }
}