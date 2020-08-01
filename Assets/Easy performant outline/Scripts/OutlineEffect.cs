using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EPOOutline
{
    public interface IGraphicsControlProvider
    {
        void SetTarget(RenderTargetIdentifier target);

        void DrawRenderer(Renderer renderer, Material material, int submesh, bool useCutoutVersion);

        void Blit(RenderTargetIdentifier first, RenderTargetIdentifier second, Material material, int passIndex);
    }

    public class OutlineParameters
    {
        public bool SustainedPerformanceMode = true;
        public IGraphicsControlProvider ControlProvider;
        public int Width;
        public int Height;
        public int Antialiasing;
        public CommandBuffer Buffer;
        public Predicate<Outlinable> DrawPredicate;
        public HashSet<int> Layers = new HashSet<int>();
        public float PrimaryRendererScale = 1.0f;
        public bool UsingInfoBuffer = false;
        public float InfoBufferScale = 1.0f;
        public int BlurIterations = 1;
        public int DilateIterations = 1;
        public float DilateShift = 1.0f;
        public float BlurShift = 1.0f;
        public RenderTargetIdentifier Target;
        public RenderTargetIdentifier Depth;
        public bool UseHDR;
        public LayerMask RenderMask;
    }

    public static class OutlineEffect
    {
        private static readonly int temporaryRT = 100;
        private static readonly int postprocessRT = 200;
        private static readonly int secondaryPostprocessRT = 300;
        private static readonly int copySurface = 400;
        private static readonly int infoBuffer = 800;
        private static readonly int secondaryInfoBuffer = 600;

        private static int infoBufferHash;
        private static int alphaTextureHash;
        private static int cutoutHash;
        private static int processDirectionHash;
        private static int colorHash;
        private static int depthHash;

        private static Material outlinePostProcessMaterial;
        private static Material blitMaterial;
        private static Material basicBlitMaterial;
        private static Material fillMaterial;
        private static Material infoMaterial;
        private static Material addAlphaBlitMaterial;
        private static Material clearStencilMaterial;

        private static List<Outlinable> outlinables = new List<Outlinable>();
        private static List<Renderer> renderers = new List<Renderer>();

        private static List<RenderInfo> targets = new List<RenderInfo>(); 

        private static RenderTextureFormat? hdrFormat = null;

        private struct RenderInfo
        {
            public readonly Renderer Renderer;
            public readonly int SubmeshesCount;
            public readonly Outlinable Outlinable;
            public readonly CutoutOutlineModule CutoutModule;

            public RenderInfo(Renderer renderer, int submeshesCount, Outlinable outlinable, CutoutOutlineModule cutoutModule)
            {
                Renderer = renderer;
                SubmeshesCount = submeshesCount;
                Outlinable = outlinable;
                CutoutModule = cutoutModule;
            }
        }

        private static void CheckIsPrepared()
        {
            depthHash = Shader.PropertyToID("_Depth");
            cutoutHash = Shader.PropertyToID("_Cutout");
            alphaTextureHash = Shader.PropertyToID("_AlphaTexture");
            colorHash = Shader.PropertyToID("_Color");
            infoBufferHash = Shader.PropertyToID("_InfoBuffer");
            processDirectionHash = Shader.PropertyToID("_OutlinePostProcessDirection");

            if (clearStencilMaterial == null)
                clearStencilMaterial = new Material(Resources.Load<Shader>("Shaders/ClearStencil"));

            if (outlinePostProcessMaterial == null)
                outlinePostProcessMaterial = Resources.Load<Material>("Outline/Materials/Outline postprocess");

            if (blitMaterial == null)
                blitMaterial = Resources.Load<Material>("Outline/Materials/Blit material");

            if (basicBlitMaterial == null)
                basicBlitMaterial = Resources.Load<Material>("Outline/Materials/Basic blit");

            if (fillMaterial == null)
                fillMaterial = Resources.Load<Material>("Outline/Materials/Blank body");

            if (infoMaterial == null)
                infoMaterial = Resources.Load<Material>("Outline/Materials/Info writer");

            if (addAlphaBlitMaterial == null)
                addAlphaBlitMaterial = Resources.Load<Material>("Outline/Materials/Add alpha blit");
        }

        private static int GetSubmeshCount(Renderer currentRenderer)
        {
            if (currentRenderer is MeshRenderer)
            {
                var filter = currentRenderer.GetComponent<MeshFilter>();
                return filter == null || filter.sharedMesh == null ? 0 : filter.sharedMesh.subMeshCount;
            }
            else if (currentRenderer is SkinnedMeshRenderer)
                return (currentRenderer as SkinnedMeshRenderer).sharedMesh.subMeshCount;
            else
                return 1;
        }

        private static void Swap(ref int a, ref int b)
        {
            var t = a;
            a = b;
            b = t;
        }

        private static void Postprocess(CommandBuffer buffer, IGraphicsControlProvider provider, Vector2 shift, int iterations, ref int firstTarget, ref int secondTarget, Material material, int pass)
        {
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                buffer.SetGlobalVector(processDirectionHash, new Vector4(shift.x / 2.0f, 0));
                provider.Blit(firstTarget, secondTarget, material, pass);
                Swap(ref firstTarget, ref secondTarget);
                buffer.SetGlobalVector(processDirectionHash, new Vector4(0, shift.y / 2.0f));
                provider.Blit(firstTarget, secondTarget, material, pass);
                Swap(ref firstTarget, ref secondTarget);
            }
        }

        private static RenderTextureFormat GetHDRFormat()
        {
            if (hdrFormat.HasValue)
                return hdrFormat.Value;

            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                hdrFormat = RenderTextureFormat.ARGBHalf;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                hdrFormat = RenderTextureFormat.ARGBFloat;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB64))
                hdrFormat = RenderTextureFormat.ARGB64;
            else
                hdrFormat = RenderTextureFormat.ARGB32;

            return hdrFormat.Value;
        }

        private static void GetTemporaryRT(OutlineParameters parameters, int id, int width, int height, int depthBuffer)
        {
            var rtFormat = parameters.UseHDR ? GetHDRFormat() : RenderTextureFormat.ARGB32;
             
            if (parameters.Antialiasing > 1)
            { 
                parameters.Buffer.GetTemporaryRT(id, width, height, depthBuffer, FilterMode.Bilinear, rtFormat,
                    RenderTextureReadWrite.Default, parameters.Antialiasing);
            }
            else
            {
                parameters.Buffer.GetTemporaryRT(id, width, height, depthBuffer, FilterMode.Bilinear, rtFormat,
                    RenderTextureReadWrite.Default);
            }
        }

        private static bool SetupCutout(OutlineParameters parameters, CutoutOutlineModule module)
        {
            if (module == null)
                return false;

            parameters.Buffer.SetGlobalTexture(alphaTextureHash, module.CutoutTexture);
            parameters.Buffer.SetGlobalFloat(cutoutHash, module.CutoutAlpha);

            return true;
        }

        private static void SetupDepth(OutlineParameters parameters)
        {
            if (parameters.Depth == BuiltinRenderTextureType.None)
                return;

            parameters.Buffer.SetGlobalTexture(depthHash, parameters.Depth);
        }

        public static void SetupBuffer(OutlineParameters parameters)
        {
            CheckIsPrepared();

            outlinables.Clear();
            Outlinable.GetAllActiveOutlinables(outlinables);
            parameters.Buffer.Clear();

            if (outlinables.Count == 0 && !parameters.SustainedPerformanceMode)
                return;

            SetupDepth(parameters);

            if (parameters.SustainedPerformanceMode && parameters.Layers.Count == 0)
                parameters.Layers.Add(int.MaxValue);

            var scaledWidth     = (int)(parameters.Width * parameters.PrimaryRendererScale);
            var scaledHeight    = (int)(parameters.Height * parameters.PrimaryRendererScale);

            foreach (var layer in parameters.Layers)
            {
                GetTemporaryRT(parameters, temporaryRT, parameters.Width, parameters.Height, 0);

                parameters.ControlProvider.Blit(parameters.Target, temporaryRT, basicBlitMaterial, 0);

                parameters.ControlProvider.SetTarget(parameters.Target);

                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);

                targets.Clear();
                foreach (var outlinable in outlinables)
                {
                    if (outlinable.Layer != layer)
                        continue;

                    if (!parameters.DrawPredicate(outlinable))
                        continue;

                    renderers.Clear();
                    outlinable.GetRenderers(renderers);
                    foreach (var currentRenderer in renderers)
                    {
                        if (currentRenderer == null || 
                            !currentRenderer.enabled || 
                            !currentRenderer.gameObject.activeInHierarchy)
                            continue;

                        if (((1 << currentRenderer.gameObject.layer) & parameters.RenderMask.value) == 0)
                            continue;

                        var submeshesCount = GetSubmeshCount(currentRenderer);
                        var ignoreModules = outlinable.ModulesInteractionApproachType == Outlinable.ModulesUsageApproach.Ignore;

                        targets.Add(new RenderInfo(currentRenderer, submeshesCount, outlinable, 
                            ignoreModules ? null : currentRenderer.GetComponent<CutoutOutlineModule>()));
                    }
                }

                foreach (var target in targets)
                {
                    var maskMaterialToUse = target.Outlinable.MaskMaterial;
                    parameters.Buffer.SetGlobalColor(colorHash, target.Outlinable.OutlineColor);

                    for (var submesh = 0; submesh < target.SubmeshesCount; submesh++)
                    {
                        if (maskMaterialToUse != null)
                            parameters.ControlProvider.DrawRenderer(target.Renderer, maskMaterialToUse, submesh, false);

                        parameters.ControlProvider.DrawRenderer(target.Renderer, target.Outlinable.OutlineMaterial, submesh, SetupCutout(parameters, target.CutoutModule));

                        if (maskMaterialToUse != null)
                            parameters.ControlProvider.DrawRenderer(target.Renderer, clearStencilMaterial, submesh, false);
                    }
                }

                // Post processing
                GetTemporaryRT(parameters, postprocessRT, scaledWidth, scaledHeight, 0);
                GetTemporaryRT(parameters, secondaryPostprocessRT, scaledWidth, scaledHeight, 0);
                GetTemporaryRT(parameters, copySurface, parameters.Width, parameters.Height, 0);

                parameters.ControlProvider.Blit(parameters.Target, copySurface, addAlphaBlitMaterial, 0);
                parameters.ControlProvider.Blit(copySurface, postprocessRT, basicBlitMaterial, -1);

                parameters.ControlProvider.SetTarget(parameters.Target);

                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);

                if (parameters.UsingInfoBuffer)
                {
                    var scaledInfoWidth = (int)(parameters.Width * parameters.InfoBufferScale);
                    var scaledInfoHeight = (int)(parameters.Height * parameters.InfoBufferScale);

                    GetTemporaryRT(parameters, infoBuffer, scaledInfoWidth, scaledInfoHeight, 0);

                    foreach (var target in targets)
                    {
                        var maskMaterialToUse = target.Outlinable.MaskMaterial;

                        parameters.Buffer.SetGlobalColor(colorHash, new Color(target.Outlinable.BlurShift, target.Outlinable.DilateShift, 0, 0));
                        for (var submesh = 0; submesh < target.SubmeshesCount; submesh++)
                        {
                            if (maskMaterialToUse != null)
                                parameters.ControlProvider.DrawRenderer(target.Renderer, maskMaterialToUse, submesh, false);

                            parameters.ControlProvider.DrawRenderer(target.Renderer, infoMaterial, submesh, false);

                            if (maskMaterialToUse != null)
                                parameters.ControlProvider.DrawRenderer(target.Renderer, clearStencilMaterial, submesh, false);
                        }
                    }

                    parameters.Buffer.SetGlobalTexture(infoBufferHash, infoBuffer);

                    parameters.ControlProvider.Blit(parameters.Target, infoBuffer, basicBlitMaterial, -1);

                    parameters.ControlProvider.SetTarget(parameters.Target);

                    GetTemporaryRT(parameters, secondaryInfoBuffer, scaledInfoWidth, scaledInfoWidth, 0);

                    var firstInfoBuffer = infoBuffer;
                    var secondInfoBuffer = secondaryInfoBuffer;
                    
                    Postprocess(parameters.Buffer, parameters.ControlProvider,
                        new Vector2(1.0f / scaledInfoWidth, 1.0f / scaledInfoHeight),
                        parameters.BlurIterations + parameters.DilateIterations,
                        ref firstInfoBuffer,
                        ref secondInfoBuffer,
                        outlinePostProcessMaterial,
                        1);
                }

                var firstTarget = postprocessRT;
                var secondTarget = secondaryPostprocessRT;

                Postprocess(parameters.Buffer,
                    parameters.ControlProvider,
                    new Vector2(parameters.DilateShift / scaledWidth, parameters.DilateShift / scaledHeight),
                    parameters.DilateIterations,
                    ref firstTarget,
                    ref secondTarget,
                    outlinePostProcessMaterial,
                    parameters.UsingInfoBuffer ? 3 : 1);

                Postprocess(parameters.Buffer,
                    parameters.ControlProvider,
                    new Vector2(parameters.BlurShift / scaledWidth, parameters.BlurShift / scaledHeight),
                    parameters.BlurIterations,
                    ref firstTarget,
                    ref secondTarget,
                    outlinePostProcessMaterial,
                    parameters.UsingInfoBuffer ? 2 : 0);

                parameters.ControlProvider.Blit(firstTarget, copySurface, blitMaterial, 0);

                parameters.Buffer.ReleaseTemporaryRT(postprocessRT);
                parameters.Buffer.ReleaseTemporaryRT(secondaryPostprocessRT);

                parameters.ControlProvider.SetTarget(copySurface);

                // Carving

                foreach (var target in targets)
                {
                    for (var submesh = 0; submesh < target.SubmeshesCount; submesh++)
                        parameters.ControlProvider.DrawRenderer(target.Renderer, fillMaterial, submesh, SetupCutout(parameters, target.CutoutModule));
                }

                parameters.ControlProvider.Blit(copySurface, temporaryRT, blitMaterial, -1);

                parameters.Buffer.ReleaseTemporaryRT(copySurface);

                parameters.ControlProvider.Blit(temporaryRT, parameters.Target, basicBlitMaterial, 0);

                parameters.Buffer.ReleaseTemporaryRT(temporaryRT);

                if (!parameters.UsingInfoBuffer)
                    continue;

                parameters.Buffer.ReleaseTemporaryRT(infoBuffer);
                parameters.Buffer.ReleaseTemporaryRT(secondaryInfoBuffer);
            }
        }
    }
}