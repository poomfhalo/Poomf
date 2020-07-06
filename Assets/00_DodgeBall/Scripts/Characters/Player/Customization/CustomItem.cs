using UnityEngine;
using Poomf.Data;

[RequireComponent(typeof(Renderer))]
public class CustomItem : MonoBehaviour
{
    // Can this item's color be adjusted?
    [SerializeField] protected bool isColorable = false;
    // Determines if this object's texture can be changed or not.
    [SerializeField] protected bool isTextureCustomizable = false;

    [Tooltip("Contains the different textures that are used to customize this item. Applied 1 at a time using an index. Leave empty if this item's textures are not customizable.")]
    [SerializeField] protected Texture2D[] itemTextures = null;
    [SerializeField] protected ItemData itemData = null;
    [SerializeField] protected CustomItemData customData = null;//TODO: to be removed, once everything is checked

    [SerializeField] ItemType m_itemType = ItemType.Head;

    public bool IsTextureCustomizable { get { return isTextureCustomizable; } protected set { isTextureCustomizable = value; } }
    public int ItemID { get; private set; }
    public ItemType itemType => m_itemType;

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
        if (isColorable)
        {
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

            //SetColors(customData.CurrentColors);
        }

        if (IsTextureCustomizable)
        {
            //SetTexture(customData.CurrentTextureIndex);
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
            Log.Error("CustomItem -> SetColors: This object is non-colorable!",gameObject);
            return;
        }

        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            // Update each material's color
            materialProperties.SetColor("_Color", colors[i]);
            meshRenderer.SetPropertyBlock(materialProperties, i);
        }
        //customData.CurrentColors = colors;
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
        if (!isColorable)
        {
            Debug.LogError("CustomItem -> SetColor: This object is non-colorable!", gameObject);
            return;
        }

        // Update the material's color
        materialProperties.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperties, materialIndex);

        //customData.CurrentColors[materialIndex] = color;
    }
    public virtual void SetTexture(int textureIndex)
    {
        if (!IsTextureCustomizable)
        {
            Log.Error("CustomItem -> SetTexture: Texture not customizable!", gameObject);
            return;
        }
        materialProperties.SetTexture("_MainTex", itemTextures[textureIndex]);
        meshRenderer.SetPropertyBlock(materialProperties);
        //customData.CurrentTextureIndex = textureIndex;
    }

    //Deprecated, because we now do it through the CharaSkinData
    /// <summary>
    /// Returns the current color of the specified material
    /// </summary>
    //public virtual Color GetColor(int materialIndex)
    //{
    //    if (!isColorable)
    //    {
    //        Log.Error("CustomItem -> GetColor: This object is non-colorable!", gameObject);
    //        return Color.white;
    //    }

    //    return customData.CurrentColors[materialIndex];
    //}
}