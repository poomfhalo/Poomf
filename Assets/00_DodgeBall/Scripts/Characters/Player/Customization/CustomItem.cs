using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

public class CustomItem : CustomItemBase
{
    [SerializeField] protected ItemDataBase itemData = null;
    public int ItemID { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        if (itemData != null)
            ItemID = itemData.ItemID;
    }
}
