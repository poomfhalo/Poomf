using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

public class CustomHead : CustomItemBase
{
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
        // Update the material's color
        materialProperties.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperties, matToCustomize);
    }
}
