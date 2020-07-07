using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuItemLocker : MenuItemBase
    {
        [SerializeField] Transform storeContent = null;
        [SerializeField] GameObject lockerItemPrefab = null;
        // TODO : For testing, to be removed.
        [SerializeField] private ItemData[] testItemData = null;
        [Header("Zoom Button")]
        [SerializeField] Image zoomButtonImage = null;
        [SerializeField] Sprite zoomInSprite = null;
        [SerializeField] Sprite zoomOutSprite = null;
        [Header("Character Customization")]
        [SerializeField] CustomizablePlayer customizablePlayer = null;
        [SerializeField] List<MenuLockerColorOption> colorControlMenus = null;
        [Header("Virtual Cameras")]
        [SerializeField] CinemachineVirtualCamera zoomedOutCamera = null;
        [SerializeField] CinemachineVirtualCamera zoomedInCamera = null;

        bool initialized = false;
        bool zoomedIn = false;
        CharaSkinData skinData => customizablePlayer.GetSkinData;

        void OnEnable()
        {
            initialize();
            if (initialized)
            {
                // Enable the zoomed out camera
                zoomedOutCamera.gameObject.SetActive(true);
                // Make sure the Zoomed in camera is disabled
                zoomedInCamera.gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            // Disable both cameras to go back to the lobby camera
            zoomedOutCamera.gameObject.SetActive(false);
            zoomedInCamera.gameObject.SetActive(false);
        }

        void initialize()
        {
            if (true == initialized) return;

            populateStoreItems();
            // Make sure the zoom button has the zoom in image
            zoomButtonImage.sprite = zoomInSprite;
            // Make sure zoomed in is false
            zoomedIn = false;
            initialized = true;

            // Initialize the color menus
            for (int i = 0; i < colorControlMenus.Count; i++)
                colorControlMenus[i].Initialize(skinData);
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

                item.InitializeItem(testItemData[i].ItemName, null, null, itemData.ItemSprite, "OWNED", itemData.ItemID);
                newItem.transform.SetParent(storeContent, false);
                item.MyButton.onClick.AddListener(OnInventoryItemButtonPressed);
            }
        }

        public void OnZoomButtonPressed()
        {
            if (zoomedIn)
            {
                // This means that we should zoom out
                zoomedInCamera.gameObject.SetActive(false);
                zoomedOutCamera.gameObject.SetActive(true);
                zoomButtonImage.sprite = zoomInSprite;
                zoomedIn = false;
            }
            else
            {
                // Zoom in
                zoomedOutCamera.gameObject.SetActive(false);
                zoomedInCamera.gameObject.SetActive(true);
                zoomButtonImage.sprite = zoomOutSprite;
                zoomedIn = true;
            }
        }

        // TODO: change to a more suitable implementation when Asset Bundles are ready
        public void OnInventoryItemButtonPressed()
        {
            // Get the currently selected inventory item
            InventoryItem selectedItem = EventSystem.current.currentSelectedGameObject.GetComponent<InventoryItem>();
            if (selectedItem != null)
            {
                // Get the item corresponding to that button
                for (int i = 0; i < testItemData.Length; i++)
                {
                    if (testItemData[i].ItemID == selectedItem.ItemID)
                    {
                        int id = testItemData[i].ItemID;
                        if (testItemData[i].ItemCategory == ItemCategory.Head)
                            //customizablePlayer.EquipHead(testItemData[i].ItemID);
                            skinData.SetItemID(ItemCategory.Head, id);
                        else if (testItemData[i].ItemCategory == ItemCategory.Outfit)
                            //customizablePlayer.EquipOutfit(testItemData[i].ItemID);
                            skinData.SetItemID(ItemCategory.Outfit, id);
                        break;
                    }
                }
            }
        }
    }
}