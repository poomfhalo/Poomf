using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVariantHead : CustomHead
{
    [Tooltip("The index of the material, among the mesh renderer's materials, that will be customized when the texture changes.")]
    [SerializeField] protected int textureMaterialIndex = 0;
    [Tooltip("Contains the different textures that are used to customize this item. Applied 1 at a time using an index.")]
    [SerializeField] protected Texture2D[] itemTextures = null;

    public override void SetTexture(int textureIndex)
    {
        materialProperties.Clear();
        if (itemTextures.Length == 0)
        {
            Debug.LogWarning("CustomVariantHead -> SetTexture : Textures array empty!");
            return;
        }
        if (textureIndex >= itemTextures.Length)
        {
            // Texture index is out of bounds! just set it to the last texture's index
            textureIndex = itemTextures.Length - 1;
        }
        materialProperties.SetTexture("_MainTex", itemTextures[textureIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, textureMaterialIndex);
    }
}
