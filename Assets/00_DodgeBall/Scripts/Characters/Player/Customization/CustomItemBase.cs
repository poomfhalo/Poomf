using UnityEngine;
using Poomf.Data;

[RequireComponent(typeof(Renderer))]
public abstract class CustomItemBase : MonoBehaviour
{
    [Tooltip("The index of the material, among the mesh renderer's materials, that will be customized when the color or texture change.")]
    [SerializeField] protected int matToCustomize = 0;

    #region Setters/Getters
    public ItemCategory itemType => m_itemType;
    #endregion

    // The children of this class will handle how itemType is initialized 
    protected ItemCategory m_itemType = ItemCategory.Head;
    // The main mesh renderer.
    protected Renderer meshRenderer = null;
    // Used to edit the properties of the renderer's materials
    protected MaterialPropertyBlock materialProperties;
    public virtual void Initialize()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (matToCustomize >= meshRenderer.materials.Length)
        {
            Debug.LogError("CustomItemBase : \"material index to customize\" is out of range! It will be set to 0.");
            matToCustomize = 0;
        }
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

    // Each of this class's children will implement the functions that it needs 

    public virtual void SetColor(Color color) { }
    // Uses a color index instead of an explicit color
    public virtual void SetColor(int colorIndex) { }
    public virtual void SetTexture(int textureIndex) { }
}