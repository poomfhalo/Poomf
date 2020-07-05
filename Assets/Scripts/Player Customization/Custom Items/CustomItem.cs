using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

[RequireComponent(typeof(Renderer))]
public class CustomItem : MonoBehaviour
{
    // Can this item's color be adjusted?
    [SerializeField] protected bool isColorable = false;

    [Tooltip("Contains the different textures that are used to customize this item. Applied 1 at a time using an index. Leave empty if the object has no textures.")]
    [SerializeField] protected Texture2D[] itemTextures = null;
    [SerializeField] protected ItemData itemData = null;
    [SerializeField] protected CustomItemData customData = null;

    // Determines if this object's texture can be changed or not. Set to false(non-customizable) if the textures array is empty.
    public bool IsTextureCustomizable { get; protected set; } = false;

    // The main mesh renderer.
    protected Renderer meshRenderer = null;
    // Used to edit the properties of the renderer's materials
    protected MaterialPropertyBlock materialProperties;
    public virtual void Initialize()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        materialProperties = new MaterialPropertyBlock();
        // Update colors
        if (isColorable)
        {
            if (customData.CurrentColors.Length < meshRenderer.sharedMaterials.Length)
            {
                // Color array sizes don't match! Either the customData isn't initialized yet, or new materials were added to the renderer
                // In case the custom data contains more colors than the renderer, the extra ones will simply be ignored
                Color[] newArr = new Color[meshRenderer.sharedMaterials.Length];
                // First, copy the existing colors in the customData. This is to ensure that when adding new materials, the old
                // Material colors are preserved
                int i = 0;
                while (i < customData.CurrentColors.Length)
                {
                    newArr[i] = customData.CurrentColors[i];
                    i++;
                }
                // Now add the new materials' colors. This will also initialize the customData's array if it's not initialized
                while (i < newArr.Length)
                {
                    newArr[i] = meshRenderer.sharedMaterials[i].color;
                    i++;
                }

                customData.CurrentColors = newArr;
            }

            SetColors(customData.CurrentColors);
        }

        if (itemTextures == null || itemTextures.Length == 0)
        {
            // This item's texture is not customizable
            IsTextureCustomizable = false;
        }
        else
        {
            IsTextureCustomizable = true;
            // Update Texture
            SetTexture(customData.CurrentTextureIndex);
        }
    }

    /// <summary>
    /// Changes the color of each material attached to the renderer following the order they are attached in.
    /// </summary>
    /// <param name="colors">
    /// Any number of colors can be passed. Extra colors will be discarded, however.
    /// </param>
    public virtual void SetColors(Color[] colors)
    {
        if (!isColorable)
        {
            Debug.LogError("CustomItem -> SetColor: This object is non-colorable!");
            return;
        }

        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            // Update each material's color
            materialProperties.SetColor("_Color", colors[i]);
            meshRenderer.SetPropertyBlock(materialProperties, i);
        }
        customData.CurrentColors = colors;
    }

    public virtual void SetTexture(int textureIndex)
    {
        if (!IsTextureCustomizable)
        {
            Debug.LogError("CustomItem -> SetTexture: Texture not customizable!");
            return;
        }
        materialProperties.SetTexture("_MainTex", itemTextures[textureIndex]);
        meshRenderer.SetPropertyBlock(materialProperties);
        customData.CurrentTextureIndex = textureIndex;
    }
}
