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
        [SerializeField] CinemachineVirtualCamera zoomedInCamera = null;
        [SerializeField] Transform headsContentParent = null;
        [SerializeField] Transform bodiesContentParent = null;
        [SerializeField] GameObject lockerItemPrefab = null;
        // TODO : For testing, to be removed.
        [SerializeField] private HeadItemData[] testItemHeads = null;
        // TODO : For testing, to be removed.
        [SerializeField] private BodyItemData[] testItemBodies = null;
        [Header("UI")]
        [SerializeField] Image zoomButtonImage = null;
        [SerializeField] Sprite zoomInSprite = null;
        [SerializeField] Sprite zoomOutSprite = null;
        // The default sprite of unequipped inventory items
        [SerializeField] Sprite defaultButtonSprite = null;
        [SerializeField] Sprite equippedButtonSprite = null;
        [Header("Character Customization")]
        [SerializeField] CustomizablePlayer customizablePlayer = null;
        [SerializeField] List<MenuLockerColorOption> colorControlMenus = null;

        InventoryItem currentHead = null;
        InventoryItem currentBody = null;
        bool initialized = false;
        bool zoomedIn = false;
        CharaSkinData skinData => customizablePlayer.GetSkinData;

        protected override void OnEnable()
        {
            initialize();
            base.OnEnable();
            // Make sure the Zoomed in camera is disabled
            zoomedInCamera.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            // Disable both cameras to go back to the lobby camera
            base.OnDisable();
            zoomedInCamera.gameObject.SetActive(false);
        }

        void initialize()
        {
            if (true == initialized) return;

            populateLockerItems();
            // Make sure the zoom button has the zoom in image
            zoomButtonImage.sprite = zoomInSprite;
            // Make sure zoomed in is false
            zoomedIn = false;
            initialized = true;

            // Initialize the color menus
            for (int i = 0; i < colorControlMenus.Count; i++)
                colorControlMenus[i].Initialize(skinData);
        }

        private void populateLockerItems()
        {
            if (null == lockerItemPrefab || null == testItemBodies || 0 == testItemBodies.Length || null == testItemHeads || 0 == testItemHeads.Length) return;

            // Initialize the bodies
            int itemsCount = testItemBodies.Length;

            for (int i = 0; i < itemsCount; i++)
            {
                BodyItemData itemData = testItemBodies[i];
                // Create an "inventory item" for each variant
                for (int j = 0; j < itemData.GetVariantsCount(); j++)
                {
                    GameObject newItem = Instantiate(lockerItemPrefab);
                    InventoryItem item = newItem.GetComponent<InventoryItem>();
                    if (null == item) return;
                    item.InitializeItem(itemData.GetVariantName(j), null, null, itemData.ItemSprite, "", itemData.ItemID, ItemCategory.Body, j);
                    if (skinData.GetItemID(ItemCategory.Body) == itemData.ItemID && j == skinData.GetTextureIndex(ItemCategory.Body))
                    {
                        // This is the currently equipped variant!
                        item.Equip(equippedButtonSprite);
                        currentBody = item;
                    }
                    newItem.transform.SetParent(bodiesContentParent, false);
                    item.MyButton.onClick.AddListener(OnInventoryItemButtonPressed);
                }

                //CurrencyType itemCurrencyType = itemData.CurrencyType;
                //int? priceCoins = null;
                //int? priceGems = null;

                /* if (itemCurrencyType == CurrencyType.COINS_GEMS)
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
                } */
            }

            // Initialize the heads
            itemsCount = testItemHeads.Length;

            for (int i = 0; i < itemsCount; i++)
            {
                HeadItemData itemData = testItemHeads[i];
                GameObject newItem = Instantiate(lockerItemPrefab);
                InventoryItem item = newItem.GetComponent<InventoryItem>();
                if (null == item) return;
                item.InitializeItem(itemData.ItemName, null, null, itemData.ItemSprite, "", itemData.ItemID, ItemCategory.Head);
                if (skinData.GetItemID(ItemCategory.Head) == itemData.ItemID)
                {
                    // This is the currently equipped head
                    item.Equip(equippedButtonSprite);
                    currentHead = item;
                }
                newItem.transform.SetParent(headsContentParent, false);
                item.MyButton.onClick.AddListener(OnInventoryItemButtonPressed);
            }
        }

        public void OnZoomButtonPressed()
        {
            if (zoomedIn)
            {
                // This means that we should zoom out
                zoomedInCamera.gameObject.SetActive(false);
                defaultCamera.gameObject.SetActive(true);
                zoomButtonImage.sprite = zoomInSprite;
                zoomedIn = false;
            }
            else
            {
                // Zoom in
                defaultCamera.gameObject.SetActive(false);
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
                if (selectedItem.ItemType == ItemCategory.Body)
                {
                    // Get the item corresponding to that button
                    for (int i = 0; i < testItemBodies.Length; i++)
                    {
                        if (testItemBodies[i].ItemID == selectedItem.ItemID)
                        {
                            skinData.SetItemID(ItemCategory.Body, testItemBodies[i].ItemID);
                            // Variants are just different textures, so set the texture index as the variant number
                            skinData.SetTextureIndex(ItemCategory.Body, selectedItem.VariantNumber);
                            // Unequip the previous item and equip the new one
                            if (currentBody != null)
                                currentBody.Unequip(defaultButtonSprite);
                            selectedItem.Equip(equippedButtonSprite);
                            currentBody = selectedItem;
                            break;
                        }
                    }
                }
                else if (selectedItem.ItemType == ItemCategory.Head)
                {
                    for (int i = 0; i < testItemHeads.Length; i++)
                    {
                        if (testItemHeads[i].ItemID == selectedItem.ItemID)
                        {
                            skinData.SetItemID(ItemCategory.Head, testItemHeads[i].ItemID);
                            // Unequip the previous item and equip the new one
                            if (currentHead != null)
                                currentHead.Unequip(defaultButtonSprite);
                            selectedItem.Equip(equippedButtonSprite);
                            currentHead = selectedItem;
                            break;
                        }
                    }
                }

            }
        }
    }
}