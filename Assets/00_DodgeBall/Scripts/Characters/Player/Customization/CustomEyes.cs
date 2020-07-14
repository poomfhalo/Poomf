using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEyes : CustomItemBase
{
    // A serialized struct that will be used to add eye textures for each eye type though inspector 
    [System.Serializable]
    private struct EyeColorTextures
    {
        public Texture2D[] colorTextures;
    }

    // An array that contains the color textures of each available eye type.
    [SerializeField] EyeColorTextures[] eyeTypesColors;

    // The current eye type index
    int eyeTypeIndex = 0;
    // The current eye color index
    int eyeColorIndex = 0;

    public override void Initialize()
    {
        m_itemType = ItemCategory.Eyes;
        base.Initialize();
    }

    /// <summary>
    /// Changes the color of the currently selected eye type
    /// </summary>
    public override void SetColor(int colorIndex)
    {
        // The number of available colors of the currently selected eye type
        int colorsCount = eyeTypesColors[eyeTypeIndex].colorTextures.Length;
        if (colorsCount == 0)
        {
            Debug.LogWarning("CustomEyes -> SetColor : Color textures array empty!");
            return;
        }
        if (colorIndex >= colorsCount)
        {
            // Color texture index is out of bounds! Just set it to the last texture's index
            colorIndex = colorsCount - 1;
        }
        materialProperties.SetTexture("_MainTex", eyeTypesColors[eyeTypeIndex].colorTextures[colorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
        eyeColorIndex = colorIndex;
    }

    /// <summary>
    /// Changes the eye type, keeping the previous color.
    /// </summary>
    public override void SetTexture(int textureIndex)
    {
        // The number of available eye types
        int eyeTypeCount = eyeTypesColors.Length;
        if (eyeTypeCount == 0)
        {
            Debug.LogWarning("CustomEyes -> SetTexture : Color textures array empty!");
            return;
        }
        if (textureIndex >= eyeTypeCount)
        {
            // Eye type index is out of bounds! Just set it to the last type's index
            textureIndex = eyeTypeCount - 1;
        }
        materialProperties.SetTexture("_MainTex", eyeTypesColors[textureIndex].colorTextures[eyeColorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
        eyeTypeIndex = textureIndex;
    }
}
