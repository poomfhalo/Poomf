using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

namespace Poomf.UI
{
    public class MenuItemLocker : MenuItemBase
    {
        [SerializeField] Transform storeContent = null;
        [SerializeField] GameObject lockerItemPrefab = null;
        // TODO : For testing, to be removed.
        [SerializeField] private ItemData[] testItemData = null;

        bool initialized = false;

        void OnEnable()
        {
            initialize();
        }

        void initialize()
        {
            if (true == initialized) return;

            populateStoreItems();
            initialized = true;
        }

        private void populateStoreItems()
        {
            if (null == lockerItemPrefab || null == testItemData || 0 == testItemData.Length) return;

            int itemsCount = testItemData.Length;

            for (int i = 0; i < itemsCount; i++)
            {
                GameObject newItem = Instantiate(lockerItemPrefab);
                InventoryItem item = newItem.GetComponent<InventoryItem>();
                if (null == item) return;
                ItemData itemData = testItemData[i];
                CurrencyType itemCurrencyType = itemData.CurrencyType;
                int? priceCoins = null;
                int? priceGems = null;

                if (itemCurrencyType == CurrencyType.COINS_GEMS)
                {
                    priceCoins = itemData.PriceCoins;
                    priceGems = itemData.PriceGems;
                }
                else if (itemCurrencyType == CurrencyType.COINS)
                {
                    priceCoins = itemData.PriceCoins;
                }
                else if (itemCurrencyType == CurrencyType.GEMS)
                {
                    priceGems = itemData.PriceGems;
                }

                item.InitializeItem(testItemData[i].ItemName, null, null, itemData.ItemSprite, "OWNED");
                newItem.transform.SetParent(storeContent, false);
            }
        }
    }
}