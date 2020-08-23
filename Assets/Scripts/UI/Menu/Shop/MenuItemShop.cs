using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;

namespace Poomf.UI
{
    public class MenuItemShop : MenuItemBase
    {
        [SerializeField] private MenuNavigator mainmenuNavigator = null;
        [SerializeField] private Transform storeContent = null;
        [SerializeField] private GameObject storeItemPrefab = null;
        // TODO : For testing, to be removed.
        [SerializeField] private ItemDataBase[] testItemData = null;

        private bool initialized = false;

        protected override void OnEnable()
        {
            initialize();
            base.OnEnable();
            if (null == mainmenuNavigator) return;
            mainmenuNavigator.enabled = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (null == mainmenuNavigator) return;
            mainmenuNavigator.enabled = true;
        }

        public void Back()
        {
            gameObject.SetActive(false);
        }

        private void initialize()
        {
            if (true == initialized) return;

            populateStoreItems();
            initialized = true;
        }

        // Shop isn't functional right now
        private void populateStoreItems()
        {
            /*if (null == storeItemPrefab || null == testItemData || 0 == testItemData.Length) return;

            int itemsCount = testItemData.Length;

            for (int i = 0; i < itemsCount; i++)
            {
                GameObject newItem = Instantiate(storeItemPrefab);
                InventoryItem item = newItem.GetComponent<InventoryItem>();
                if (null == item) return;
                ItemDataBase itemData = testItemData[i];
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

                item.InitializeItem(testItemData[i].ItemName, priceCoins, priceGems, itemData.ItemSprite, null, itemData.ItemID, itemData.ItemCategory);
                newItem.transform.SetParent(storeContent, false); 
        } */
        }
    }
}