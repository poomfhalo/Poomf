using UnityEngine;
using System;

namespace Poomf.Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName = "Item_Name";
        [SerializeField] private int coinsPrice = 0;
        [SerializeField] private int gemsPrice = 0;
        [SerializeField] private Sprite itemSprite = null;
        [SerializeField] private CurrencyType currencyType = CurrencyType.COINS;
        [SerializeField] private ItemCategory itemCategory = ItemCategory.HEAD;
        [SerializeField] private ItemRarity itemRarity = ItemRarity.COMMON;

        [HideInInspector] [SerializeField] private int itemID = -1;

        public string ItemName { get { return ItemName; } }
        public int Price { get { return Price; } }
        public CurrencyType CurrencyType { get { return currencyType; } }
        public ItemCategory ItemCategory { get { return itemCategory; } }
        public int ItemID { get { return itemID; } }

        public bool SetItemID(int i_itemID)
        {
            if (-1 != itemID)
            {
                Debug.LogWarning("ItemData::SetItemID -> This item already has a unique ID: " + itemID);
                return false;
            }

            itemID = i_itemID;

            Debug.Log("ItemData::SetItemID -> A unique ID was assigned to this item: " + itemID);

            return true;
        }
    }
}