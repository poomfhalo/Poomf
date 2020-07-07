using UnityEngine;
using Poomf.Data;

[RequireComponent(typeof(Renderer))]
public class CustomItem : MonoBehaviour
{
    [Tooltip("Contains the different textures that are used to customize this item. Applied 1 at a time using an index. Leave empty if this item's textures are not customizable.")]
    [SerializeField] protected Texture2D[] itemTextures = null;
    [SerializeField] protected ItemData itemData = null;

    [SerializeField] ItemCategory m_itemType = ItemCategory.Head;

    #region Setters/Getters
    public int ItemID { get; private set; }
    public ItemCategory itemType => m_itemType;
    #endregion

    // The main mesh renderer.
    protected Renderer meshRenderer = null;
    // Used to edit the properties of the renderer's materials
    protected MaterialPropertyBlock materialProperties;
    public virtual void Initialize()
    {
        // During development, some items may not have item data!
        if (itemData != null)
            ItemID = itemData.ItemID;
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        materialProperties = new MaterialPropertyBlock();
        // Update colors
        //if (isColorable)
        //{
        //if (customData.CurrentColors.Length < meshRenderer.sharedMaterials.Length)
        //{
        //    // Color array sizes don't match! Either the customData isn't initialized yet, or new materials were added to the renderer
        //    // In case the custom data contains more colors than the renderer, the extra ones will simply be ignored
        //    Color[] newArr = new Color[meshRenderer.sharedMaterials.Length];
        //    // First, copy the existing colors in the customData. This is to ensure that when adding new materials, the old
        //    // Material colors are preserved
        //    int i = 0;
        //    while (i < customData.CurrentColors.Length)
        //    {
        //        newArr[i] = customData.CurrentColors[i];
        //        i++;
        //    }
        //    // Now add the new materials' colors. This will also initialize the customData's array if it's not initialized
        //    while (i < newArr.Length)
        //    {
        //        newArr[i] = meshRenderer.sharedMaterials[i].color;
        //        i++;
        //    }

        //    customData.CurrentColors = newArr;
        //}

        //}
    }

    /// <summary>
    /// Changes the color of each material attached to the renderer following the order they are attached in.
    /// </summary>
    /// <param name="colors">
    /// Any number of colors can be passed. Extra colors will be discarded, however.
    /// </param>
    public virtual void SetColors(Color[] colors)
    {
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            // Update each material's color
            materialProperties.SetColor("_Color", colors[i]);
            meshRenderer.SetPropertyBlock(materialProperties, i);
        }
    }
    /// <summary>
    /// Sets the color of a specific material
    /// </summary>
    /// <param name="color">
    /// The new color
    /// </param>
    /// <param name="materialIndex">
    /// The index of the material that we want to change its color
    /// </param>
    public virtual void SetColor(Color color, int materialIndex)
    {
        // Update the material's color
        materialProperties.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperties, materialIndex);
    }
    public virtual void SetTexture(int textureIndex)
    {
        if (itemTextures.Length == 0)
        {
            Debug.LogWarning("CustomItem -> SetTexture : Textures array empty!");
            return;
        }
        if (textureIndex >= itemTextures.Length)
        {
            // Texture index is out of bounds! just set it to the last texture's index
            textureIndex = itemTextures.Length - 1;
        }
        materialProperties.SetTexture("_MainTex", itemTextures[textureIndex]);
        meshRenderer.SetPropertyBlock(materialProperties);
    }
}