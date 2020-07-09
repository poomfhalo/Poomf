using UnityEngine;
using System;

namespace Poomf.Data
{
    //[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Items/ItemData", order = 0)]
    public class ItemDataBase : ScriptableObject
    {
        [SerializeField] private string itemName = "Item_Name";
        [SerializeField] private int coinsPrice = 0;
        [SerializeField] private int gemsPrice = 0;
        [SerializeField] private Sprite itemSprite = null;
        [SerializeField] private CurrencyType currencyType = CurrencyType.COINS;
        [SerializeField, HideInInspector] private ItemCategory itemCategory = ItemCategory.Head;
        [SerializeField] private ItemRarity itemRarity = ItemRarity.COMMON;
        [SerializeField] private ItemSet itemSet = ItemSet.SCHOOL;
        [SerializeField] private Gender itemGender = Gender.UNISEX;

        [SerializeField] private int itemID = -1;

        public string ItemName { get { return itemName; } }
        public int PriceCoins { get { return coinsPrice; } }
        public int PriceGems { get { return gemsPrice; } }
        public CurrencyType CurrencyType { get { return currencyType; } }
        public ItemCategory ItemCategory { get { return itemCategory; } protected set { itemCategory = value; } }
        public ItemRarity ItemRarity { get { return itemRarity; } }
        public ItemSet ItemSet { get { return itemSet; } }
        public Sprite ItemSprite { get { return itemSprite; } }
        public int ItemID { get { return itemID; } }

        public bool SetItemID(int i_itemID, bool i_debug = false)
        {
            if (-1 != itemID)
            {
                if (true == i_debug)
                    Debug.LogWarning("ItemData::SetItemID -> This item already has a unique ID: " + itemID);

                return false;
            }

            itemID = i_itemID;

            if (true == i_debug)
                Debug.Log("ItemData::SetItemID -> A unique ID was assigned to this item: " + itemID);

            return true;
        }
    }
}