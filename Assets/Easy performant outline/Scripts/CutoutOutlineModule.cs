using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPOOutline
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class CutoutOutlineModule : Module
    {
        enum MaterialAccessMode
        {
            Shared,
            Instance
        }

        [SerializeField]
        [HideInInspector]
        private Texture cutoutTexture;

        [SerializeField]
        [HideInInspector]
        private string textureId = "_MainTex";

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float cutoutAlpha = 0.5f;

        [SerializeField]
        private MaterialAccessMode materialAccessMode;

        private Renderer renderer;

        public float CutoutAlpha
        {
            get
            {
                return cutoutAlpha;
            }

            set
            {
                cutoutAlpha = value;
            }
        }

        public Texture CutoutTexture
        {
            get
            {
                if (cutoutTexture != null)
                    return cutoutTexture;

                if (renderer is SpriteRenderer)
                    return (renderer as SpriteRenderer).sprite.texture;
                else
                {
                    Material materialToUse = null;
                    if (materialAccessMode == MaterialAccessMode.Shared || !Application.isPlaying)
                        materialToUse = renderer.sharedMaterial;
                    else
                        materialToUse = renderer.material;

                    if (materialToUse == null)
                        return null;

                    if (!materialToUse.HasProperty(textureId))
                        return null;

                    return materialToUse.GetTexture(textureId);
                }
            }

            set
            {
                cutoutTexture = value;
            }
        }

        private void Awake()
        {
            renderer = GetComponent<Renderer>();
        }

        private void OnDestroy()
        {
            
        }
    }
}