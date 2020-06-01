using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;

namespace GW_Lib.Utility
{
    [ExecuteAlways]
    public class ImageRendRelayerManager : MonoBehaviour
    {
        [Tooltip("Used and modified in inspector")]
        [SerializeField] GameObject[] effectsObjs = new GameObject[0];
        [SerializeField] bool workInEditor = true;

        [Header("Read Only")]
        [Tooltip("Used In Script, to refresh relayers")]
        [SerializeField] GameObject[] usableEffectsObjs = new GameObject[0];
        [Tooltip("The effects group per object, first index is number of gameObjects")]
        [SerializeField] MonoBehaviour[][] storedEffectsPerObj = new MonoBehaviour[0][];

        private void Awake()
        {
            usableEffectsObjs = new GameObject[0];
            RefreshRelayers();
        }

        #if UNITY_EDITOR
        private void LateUpdate()
        {
            if(Application.isEditor && workInEditor == false)
            {
                return;
            }
            if(ShouldRefresh())
            {
                RefreshRelayers();
            }
        }
        #endif
        public void RefreshRelayers()
        {
            ClearEffectsRelayers();
            SetUpEffectsRelaying();
        }

        private bool ShouldRefresh()
        {
            bool areSameGameObjs = Extentions.AreArraysEqual(effectsObjs,usableEffectsObjs);
            if(areSameGameObjs)
            {
                bool matchStoredFXToUsableObjs = storedEffectsPerObj.Length == usableEffectsObjs.Length;
                if(matchStoredFXToUsableObjs == false)
                {
                    return true;
                }
                for(int i = 0;i<usableEffectsObjs.Length;i++)
                {
                    bool areSameEffects = true;
                    GameObject effectsObj = usableEffectsObjs[i];
                    if(effectsObj != null)
                    {
                        List<MonoBehaviour> currEnabledEffectsL = effectsObj.GetComponents<MonoBehaviour>().ToList();
                        currEnabledEffectsL.RemoveAll((effect)=>effect.enabled==false);

                        MonoBehaviour[] currEnabledEffects = currEnabledEffectsL.ToArray();
                        MonoBehaviour[] storedEffects = storedEffectsPerObj[i];
                        areSameEffects = Extentions.AreArraysEqual(storedEffects, currEnabledEffects);
                    }
                    if(areSameEffects)
                    {
                        continue;
                    }
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        private void ClearEffectsRelayers()
        {
            foreach (ImageRendRelayer relayer in GetComponents<ImageRendRelayer>())
            {
                if(Application.isPlaying)
                {
                    Destroy(relayer);
                }
                else
                {
                    DestroyImmediate(relayer);
                }
            }
        }
        private void SetUpEffectsRelaying()
        {
            usableEffectsObjs = new GameObject[effectsObjs.Length];
            storedEffectsPerObj = new MonoBehaviour[usableEffectsObjs.Length][];

            for(int i = 0;i<usableEffectsObjs.Length;i++)
            {
                usableEffectsObjs[i] = effectsObjs[i];
                CreateAndStoreEffects(i);
            }
        }
        private void CreateAndStoreEffects(int objIndex)
        {
            GameObject g = usableEffectsObjs[objIndex];
            if(g==null)
            {
                return;
            }
            List<MonoBehaviour> enabledEffects = g.GetComponents<MonoBehaviour>().ToList();
            enabledEffects.RemoveAll((effect) => effect.enabled == false);
            foreach(MonoBehaviour effect in enabledEffects)
            {
                ImageRendRelayer relayer = gameObject.AddComponent<ImageRendRelayer>();
                relayer.TakeEffect(effect,effect.GetType());
            }
            storedEffectsPerObj[objIndex] = enabledEffects.ToArray();
        }
    }
}