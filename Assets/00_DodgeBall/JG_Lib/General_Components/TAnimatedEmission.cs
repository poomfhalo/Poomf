using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class TAnimatedEmission : MonoBehaviour
    {
        [SerializeField] Renderer[] renderers = null;
        [SerializeField] string[] emissionColorProps = { "_EmissionColor", "_LavaEmissionColor", "_RimColor" };
        [SerializeField] bool updateStartingColorsOnEnd = false;

        [Header("Read Only")]
        [SerializeField] Color startColor = Color.black;
        [SerializeField] float currF = 0;

        [SerializeField] float extraIntensity = 0;
        [SerializeField] bool colorAnimIsTint = false;
        [SerializeField] Gradient colorAnimator = null;
        [SerializeField] bool isRunning = false;
        [SerializeField] ColorAnimType colorAnimType = ColorAnimType.None;
        [SerializeField] Color targetColor = Color.clear;


        Tweener tween = null;
        enum ColorAnimType { None, Gradient, Direct }
        struct MatColorPair
        {
            public Material mat;
            public Color startColor;
            public string[] emissionColorProps;
            public MatColorPair(Material m,string[] emissionColorProps)
            {
                this.mat = m;
                this.startColor = Color.black;
                this.emissionColorProps = emissionColorProps;
                startColor = GetColor();
            }
            private Color GetColor()
            {
                foreach (var p in emissionColorProps)
                {
                    if (!mat.HasProperty(p))
                    {
                        continue;
                    }
                    return mat.GetColor(p);
                }
                Debug.Log(mat.name + " Could not find any emissionable material on ", mat);
                return Color.black;
            }
            private void SetColor(Color newCol)
            {
                foreach (var p in emissionColorProps)
                {
                    if (!mat.HasProperty(p))
                    {
                        continue;
                    }
                    mat.SetColor(p, newCol);
                    return;
                }
                Debug.Log(mat.name + " Could not find any emissionable property", mat);
            }

            public void SetMatColor(Gradient colorAnimator,bool isTint,float extraIntensity,float tweenStage)
            {
                Color newCol = colorAnimator.Evaluate(tweenStage);

                newCol = UpdateByTint(isTint, newCol);
                newCol = UpdateByIntensity(extraIntensity, tweenStage, newCol);
                SetColor(newCol);
            }
            public void SetMatColor(Color targetColor,bool isTint,float extraIntensity,float tweenStage)
            {
                Color newCol = Color.Lerp(startColor, targetColor, tweenStage);

                newCol = UpdateByTint(isTint, newCol);
                newCol = UpdateByIntensity(extraIntensity, tweenStage, newCol);
                SetColor(newCol);
            }
            public void SetMatColor(float extraIntensity,float tweenStage)
            {
                Color newColor = startColor;
                newColor = UpdateByIntensity(extraIntensity, tweenStage, newColor);
                SetColor(newColor);
            }

            private Color UpdateByTint(bool isTint,Color newColor)
            {
                if (isTint)
                {
                    return startColor * newColor;
                }
                return newColor;
            }
            private Color UpdateByIntensity(float extraIntensity,float pNewValue,Color color)
            {
                Color targetColor = color * extraIntensity;
                Color newColor = Color.Lerp(startColor, targetColor, pNewValue);
                return newColor;
                //float scaler = Mathf.LerpUnclamped(1, 1 + extraIntensity, pNewValue);
                //return color * scaler;
            }
        }

        Dictionary<Renderer, List<MatColorPair>> animatableData = new Dictionary<Renderer, List<MatColorPair>>();

        void Reset()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
        void Start()
        {
            UpdateStartingColors();
        }
        private void UpdateStartingColors()
        {
            foreach (var r in renderers)
            {
                List<MatColorPair> colors = new List<MatColorPair>();
                foreach (var mat in r.materials)
                {
                    MatColorPair pair = new MatColorPair(mat, emissionColorProps);
                    colors.Add(pair);
                }
                animatableData[r] = colors;
            }
        }

        public void KillActive()
        {
            if (!isRunning)
            {
                return;
            }
            tween.Kill();
            tween = null;
            isRunning = false;
            ColorSetter(0);
            if (updateStartingColorsOnEnd)
            {
                UpdateStartingColors();
            }
        }
        public void AnimateIntensity(float extraIntensity, float time, Ease ease, LoopType loop, int loops, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }
            colorAnimType = ColorAnimType.None;
            isRunning = true;
            this.extraIntensity = extraIntensity;
            colorAnimIsTint = false;
            currF = 0;
            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease)
            .SetLoops(loops, loop).OnComplete(() => {
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }
        public void AnimateIntensity(float extraIntensity, float time, Ease ease, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }

            colorAnimType = ColorAnimType.None;
            isRunning = true;
            colorAnimIsTint = false;
            this.extraIntensity = extraIntensity;
            currF = 0;

            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease).OnComplete(() => {
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }
        public void AnimateColor(Gradient colorAnim, float time, Ease ease, LoopType loop, int loops, bool isTint,float extraIntensity, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }
            colorAnimType = ColorAnimType.Gradient;
            isRunning = true;
            colorAnimator = colorAnim;
            colorAnimIsTint = isTint;
            this.extraIntensity = extraIntensity;
            currF = 0;

            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease)
            .SetLoops(loops, loop).OnComplete(()=> { 
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }
        public void AnimateColor(Gradient colorAnim,float time,Ease ease, bool isTint,float extraIntensity, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }
            colorAnimType = ColorAnimType.Gradient;
            isRunning = true;
            colorAnimator = colorAnim;
            colorAnimIsTint = isTint;
            this.extraIntensity = extraIntensity;
            currF = 0;

            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease).OnComplete(() => {
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }
        public void AnimateColorTo(Color col, float time, Ease ease, bool isTint, float extraIntensity, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }
            colorAnimType = ColorAnimType.Direct;
            isRunning = true;
            targetColor = col;
            colorAnimIsTint = isTint;
            this.extraIntensity = extraIntensity;
            currF = 0;

            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease).OnComplete(() => {
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }
        public void AnimateColorTo(Color col, float time, Ease ease, bool isTint, float extraIntensity,LoopType loopType, int loops, Action isDone)
        {
            if (isRunning && tween != null)
            {
                tween.Kill();
            }
            colorAnimType = ColorAnimType.Direct;
            isRunning = true;
            targetColor = col;
            colorAnimIsTint = isTint;
            this.extraIntensity = extraIntensity;
            currF = 0;

            tween = DOTween.To(ColorGetter, ColorSetter, 1, time).SetEase(ease).SetLoops(loops,loopType).OnComplete(() => {
                isRunning = false;
                isDone?.Invoke();
                if (updateStartingColorsOnEnd)
                {
                    UpdateStartingColors();
                }
            });
        }

        private void ColorSetter(float pNewValue)
        {

            foreach (var data in animatableData)
            {
                foreach (var pair in data.Value)
                {
                    switch (colorAnimType)
                    {
                        case ColorAnimType.Direct:
                            pair.SetMatColor(targetColor, colorAnimIsTint, extraIntensity, pNewValue);
                            break;
                        case ColorAnimType.Gradient:
                            pair.SetMatColor(colorAnimator, colorAnimIsTint, extraIntensity, pNewValue);
                            break;
                        case ColorAnimType.None:
                            pair.SetMatColor(extraIntensity, pNewValue);
                            break;
                    }
                }
            }

            currF = pNewValue;
        }

        private float ColorGetter()
        {
            return currF;
        }
    }
}