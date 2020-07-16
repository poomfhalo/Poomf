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
        public Texture2D blinkTexture;
    }

    // An array that contains the color textures of each available eye type.
    [SerializeField] EyeColorTextures[] eyeTypesInfo = null;
    [Header("Blinking")]
    [SerializeField] float blinkDuration = 0.1f;
    [SerializeField] float minTimeBetBlinks = 5f;
    [SerializeField] float maxTimeBetBlinks = 15f;
    // The current eye type index
    int eyeTypeIndex = 0;
    // The current eye color index
    int eyeColorIndex = 0;
    float timer = 0f;

    private void Update()
    {
        // Blinking
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                // Start the blinking coroutine
                StartCoroutine(Blink());
                // Start the timer again
                timer = Random.Range(minTimeBetBlinks, maxTimeBetBlinks);
            }
        }
    }

    public override void Initialize()
    {
        m_itemType = ItemCategory.Eyes;
        base.Initialize();
        // Start blinking. Give a random value to the timer.
        timer = Random.Range(minTimeBetBlinks, maxTimeBetBlinks);
    }

    /// <summary>
    /// Changes the color of the currently selected eye type
    /// </summary>
    public override void SetColor(int colorIndex)
    {
        // The number of available colors of the currently selected eye type
        int colorsCount = eyeTypesInfo[eyeTypeIndex].colorTextures.Length;
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
        materialProperties.SetTexture("_MainTex", eyeTypesInfo[eyeTypeIndex].colorTextures[colorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
        eyeColorIndex = colorIndex;
    }

    /// <summary>
    /// Changes the eye type, keeping the previous color.
    /// </summary>
    public override void SetTexture(int textureIndex)
    {
        // The number of available eye types
        int eyeTypeCount = eyeTypesInfo.Length;
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
        materialProperties.SetTexture("_MainTex", eyeTypesInfo[textureIndex].colorTextures[eyeColorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
        eyeTypeIndex = textureIndex;
    }

    IEnumerator Blink()
    {
        // Set the texture to the current eye type's blink texture
        materialProperties.SetTexture("_MainTex", eyeTypesInfo[eyeTypeIndex].blinkTexture);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
        // Wait for a while
        yield return new WaitForSeconds(blinkDuration);
        // Change the texture back
        materialProperties.SetTexture("_MainTex", eyeTypesInfo[eyeTypeIndex].colorTextures[eyeColorIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
    }
}
