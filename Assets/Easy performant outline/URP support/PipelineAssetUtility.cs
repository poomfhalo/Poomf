using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if SRP_OUTLINE && UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif
#endif

#if SRP_OUTLINE && UNITY_EDITOR
namespace EPOOutline
{
    public static class PipelineAssetUtility
    {
        public static RenderPipelineAsset CurrentAsset
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                var pipeline = QualitySettings.renderPipeline;
                if (pipeline == null)
                    pipeline = GraphicsSettings.renderPipelineAsset;
#else
                var pipeline = GraphicsSettings.renderPipelineAsset;
#endif

                return pipeline;
            }
        }

        public static HashSet<RenderPipelineAsset> ActiveAssets
        {
            get
            {
                var set = new HashSet<RenderPipelineAsset>();

                if (GraphicsSettings.renderPipelineAsset != null)
                    set.Add(GraphicsSettings.renderPipelineAsset);

#if UNITY_2019_3_OR_NEWER
                var qualitySettingNames = QualitySettings.names;
                for (var index = 0; index < qualitySettingNames.Length; index++)
                {
                    var assset = QualitySettings.GetRenderPipelineAssetAt(index);
                    if (assset == null)
                        continue;

                    set.Add(assset);
                }
#endif

                return set;
            }
        }

        public static RenderPipelineAsset CreateAsset(ForwardRendererData data)
        {
#if UNITY_2019_3_OR_NEWER
            return UniversalRenderPipelineAsset.Create(data);
#else
            var asset = LightweightRenderPipelineAsset.Create();

            using (var so = new SerializedObject(asset))
            {
                so.Update();
                  
                var rendererProperty = so.FindProperty("m_RendererData");
                var rendererTypeProperty = so.FindProperty("m_RendererType");
                rendererTypeProperty.enumValueIndex = (int)RendererType.Custom;

                rendererProperty.objectReferenceValue = data;

                so.ApplyModifiedProperties();
            }

            return asset;
#endif
        }

        public static ForwardRendererData CreateRenderData()
        {
            return ScriptableObject.CreateInstance<ForwardRendererData>();
        }

        public static bool IsURPOrLWRP(RenderPipelineAsset asset)
        {
            return asset != null &&
                (asset.GetType().Name.Equals("LightweightRenderPipelineAsset") ||
                asset.GetType().Name.Equals("UniversalRenderPipelineAsset"));
        }

        public static ScriptableRendererData GetRenderer(RenderPipelineAsset asset)
        {
            using (var so = new SerializedObject(asset))
            {
                so.Update();

#if SRP_OUTLINE
#if UNITY_2019_3_OR_NEWER
                var rendererDataList = so.FindProperty("m_RendererDataList");
                var assetIndex = so.FindProperty("m_DefaultRendererIndex");
                var item = rendererDataList.GetArrayElementAtIndex(assetIndex.intValue);

                return item.objectReferenceValue as ScriptableRendererData;
#else
                var rendererData = so.FindProperty("m_RendererData");
                return rendererData.objectReferenceValue as ScriptableRendererData;
#endif
#else
                return null;
#endif
            }
        }

        public static bool IsAssetContainsSRPOutlineFeature(RenderPipelineAsset asset)
        {
            var data = GetRenderer(asset);

            return data.rendererFeatures.Find(x => x is SRPOutlineFeature) != null;
        }
    }
}
#endif