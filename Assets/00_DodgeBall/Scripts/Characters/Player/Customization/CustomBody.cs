using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

public class CustomBody : CustomItemBase
{
    [Tooltip("Contains the different textures that are used to customize this item. Applied 1 at a time using an index.")]
    [SerializeField] protected Texture2D[] itemTextures = null;
    [SerializeField] protected ItemDataBase itemData = null;
    public int ItemID { get; private set; }

    public override void Initialize()
    {
        m_itemType = ItemCategory.Body;
        base.Initialize();
        if (itemData != null)
            ItemID = itemData.ItemID;
    }

    public override void SetTexture(int textureIndex)
    {
        if (itemTextures.Length == 0)
        {
            Debug.LogWarning("CustomBody -> SetTexture : Textures array empty!");
            return;
        }
        if (textureIndex >= itemTextures.Length)
        {
            // Texture index is out of bounds! just set it to the last texture's index
            textureIndex = itemTextures.Length - 1;
        }
        materialProperties.SetTexture("_MainTex", itemTextures[textureIndex]);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
    }
}
