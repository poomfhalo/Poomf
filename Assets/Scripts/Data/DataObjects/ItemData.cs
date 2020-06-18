using UnityEngine;
using System;

namespace Poomf.Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
    public class ItemData : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string itemName = "Item_Name";
        [SerializeField] private string itemID = "Item_Id";
        [SerializeField] private int price = 0;
        [SerializeField] private CurrencyType currencyType = CurrencyType.COINS;
        [SerializeField] private ItemCategory itemCategory = ItemCategory.HEAD;

        [NonSerialized] private int itemHashID = 0;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            itemHashID = itemID.GetHashCode();
        }

        public string ItemName { get { return ItemName; } }
        public int Price { get { return Price; } }
        public CurrencyType CurrencyType { get { return currencyType; } }
        public ItemCategory ItemCategory { get { return itemCategory; } }
        public int ItemHashID { get { return itemHashID; } }
    }
}