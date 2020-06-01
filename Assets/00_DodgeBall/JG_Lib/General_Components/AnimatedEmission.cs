using System.Collections.Generic;
using StealthGame.Utility;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class AnimatedEmission : MonoBehaviour
    {
        [System.Serializable]
        struct AnimatedMatData
        {
            public Material mat;
            public Color[] startColors;
            public int[] existingColorProps;
            public AnimatedMatData(Material mat,Color[] startColors,int[] existingColorProps)
            {
                this.mat=mat;
                this.startColors = startColors;
                this.existingColorProps = existingColorProps;
            }
        }

        [Header("Core Settings")]
        [SerializeField] MinMaxRange rDuration = new MinMaxRange(0,3,2,3);
        [SerializeField] Renderer[] animatableRenderers = new Renderer[0];
        [SerializeField] AnimMode animMode = AnimMode.PingPong;
        [SerializeField] string[] emissionColorProps = {"_EmissionColor","_LavaEmissionColor","_RimColor"};

        [Header("Intensity Anim Settings")]
        [SerializeField] bool animateIntensity = true;
        [SerializeField] AnimationCurve intensityOverLifeTime = new AnimationCurve(new Keyframe(0.0f, 0.0f),
                new Keyframe(0.5f, 1.0f),new Keyframe(1.0f, 0.0f));
        [SerializeField] MinMaxRange curveMultiplayer = new MinMaxRange(0,3,0.5f,1.5f);

        [Header("Color Anim Settings")]
        [Tooltip("if this is true, then the 'Gradient' will only act as tint (multiplied by start color)\n" +
            "if this is false, the color will be set without regard to start color")]
        [SerializeField] bool colorOverLifeTimeIsTint = true;
        [SerializeField] bool animateColor = false;
        [SerializeField] Gradient colorOverLifeTime = new Gradient();

        enum AnimMode{PingPong,OneWay,Loop}

        List<AnimatedMatData> matsData = new List<AnimatedMatData>();
        float time = 0;
        float duration = 0;
        private void Reset()
        {
            animatableRenderers = GetComponentsInChildren<Renderer>();
        }
        private void Awake()
        {
            duration = rDuration.GetValue();
            Keyframe[] keys = intensityOverLifeTime.keys;
            float curveMultiplayerVal = curveMultiplayer.GetValue();
            for (int i = 0;i<keys.Length;i++)
            {
                Keyframe key = keys[i];
                key.value = key.value * curveMultiplayerVal;
                keys[i] = key;
            }
            intensityOverLifeTime = new AnimationCurve(keys);
            RefreshMaterials();
        }
        private void Update()
        {
            if(UpdateTime () == false)
            {
                return;
            }

            foreach(AnimatedMatData mat in matsData)
            {
                for (int i =0;i<mat.existingColorProps.Length;i++)
                {
                    int existingPropIndex = mat.existingColorProps[i];
                    Color startColor = mat.startColors[i];
                    string p = emissionColorProps[existingPropIndex];

                    mat.mat.SetColor(p,UpdateColor(startColor));
                }
            }
        }

        private bool UpdateTime()
        {
            switch(animMode)
            {
                case AnimMode.PingPong:
                    time = Mathf.PingPong(Time.time,duration);
                    return true;
                case AnimMode.OneWay:
                    if(time>duration)
                    {
                        return false;
                    }
                    time=time+Time.deltaTime;
                    return true;
                case AnimMode.Loop:
                    time = time+Time.deltaTime;
                    if(time>duration)
                    {
                        time=0;
                    }
                    return true;
            }
            return false;
        }
        private Color UpdateColor(Color startColor)
        {
            float normTime = time/duration;
            Color newColor = startColor;
            if(animateColor)
            {
                if (colorOverLifeTimeIsTint)
                {
                    newColor = newColor * colorOverLifeTime.Evaluate(normTime);
                }
                else
                {
                    newColor = colorOverLifeTime.Evaluate(normTime);
                }
            }
            if(animateIntensity)
            {
                newColor = newColor * intensityOverLifeTime.Evaluate(normTime);
            }
            return newColor;
        }

        private void RefreshMaterials()
        {
            matsData = new List<AnimatedMatData>();
            for (int i = 0; i < animatableRenderers.Length; i++)
            {
                Material[] mats = animatableRenderers[i].materials;
                for (int j=0;j<mats.Length;j++)
                {
                    List<int> existingProps = new List<int>();
                    List<Color> startColors = new List<Color>();

                    for (int k = 0;k<emissionColorProps.Length;k++)
                    {
                        string p = emissionColorProps[k];
                        if (mats[j].HasProperty(p) == false)
                        {
                            continue;
                        }

                        Color startCol = mats[j].GetColor(p);
                        existingProps.Add(k);
                        startColors.Add(startCol);
                    }
                    matsData.Add(new AnimatedMatData(mats[j],startColors.ToArray(),existingProps.ToArray()));
                }
            }
        }

    }
}