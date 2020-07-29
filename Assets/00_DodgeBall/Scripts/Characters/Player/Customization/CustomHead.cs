using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

public class CustomHead : CustomItemBase
{
    [Tooltip("The index of the material, among the mesh renderer's materials, that will be customized when the color changes.")]
    [SerializeField] protected int colorMaterialIndex = 0;
    [SerializeField] protected ItemDataBase itemData = null;
    public int ItemID { get; private set; }

    public override void Initialize()
    {
        m_itemType = ItemCategory.Head;
        base.Initialize();
        if (itemData != null)
            ItemID = itemData.ItemID;
    }

    public override void SetColor(Color color)
    {
        materialProperties.Clear();
        // Update the material's color
        materialProperties.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperties, colorMaterialIndex);
    }
}
