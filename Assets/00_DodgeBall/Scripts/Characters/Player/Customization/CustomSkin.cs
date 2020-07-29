using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSkin : CustomItemBase
{
    [Tooltip("The index of the material, among the mesh renderer's materials, that will be customized when the color changes.")]
    [SerializeField] protected int colorMaterialIndex = 0;
    [SerializeField] protected Texture2D[] skinColorTextures = null;
    public override void Initialize()
    {
        m_itemType = ItemCategory.Skin;
        base.Initialize();
    }

    public override void SetColor(int colorIndex)
    {
        materialProperties.Clear();
        if (skinColorTextures.Length == 0)
        {
            Debug.LogWarning("CustomSkin -> SetColor : Color textures array empty!");
            return;
        }
        if (colorIndex >= skinColorTextures.Length)
        {
            // Texture index is out of bounds! just set it to the last texture's index
            colorIndex = skinColorTextures.Length - 1;
        }
        materialProperties.SetTexture("_MainTex", skinColorTextures[colorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, colorMaterialIndex);
    }
}
