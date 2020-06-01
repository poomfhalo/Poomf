using System;
using System.Reflection;
using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteInEditMode]
    public class ImageRendRelayer : MonoBehaviour
    {
        [Header("Read Only")]
        [SerializeField] MonoBehaviour caller = null;
        [SerializeField] MethodInfo method = null;

        void OnRenderImage(RenderTexture source, RenderTexture destionation)
        {
            if(caller == null)
            {
                Graphics.Blit(source, destionation);
                return;
            }
            if(Application.isPlaying && method!=null)
            {
                method.Invoke(caller, new object[]{source, destionation});
            }
            else if(Application.isPlaying == false && GetMethod(caller.GetType()) != null)
            {
                GetMethod(caller.GetType()).Invoke(caller,new object[]{source, destionation});
            }
            else
            {
                Graphics.Blit(source, destionation);
            }
        }
        #if UNITY_EDITOR
        private void Update()
        {
            if(caller == null || caller.enabled == false)
            {
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }
            }
        }
        #endif
        public void TakeEffect(MonoBehaviour caller,Type type)
        {
            this.caller = caller;
            method = GetMethod(type);
        }
        private MethodInfo GetMethod(Type type)
        {
            return type.GetMethod("OnRenderImage",BindingFlags.NonPublic | BindingFlags.Instance);
        }

    }
}