using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuItemLocker : MenuItemInventory
    {
        [SerializeField] CinemachineVirtualCamera zoomedInCamera = null;
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
        bool populated = false;

        CharaSkinData skinData => customizablePlayer.GetSkinData;

        protected override void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override void OnEnable()
        {
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

            // Make sure the zoom button has the zoom in image
            zoomButtonImage.sprite = zoomInSprite;
            // Make sure zoomed in is false
            zoomedIn = false;
            initialized = true;

            // Initialize the color menus
            for (int i = 0; i < colorControlMenus.Count; i++)
                colorControlMenus[i].Initialize(skinData);
        }

        protected override void Populate()
        {
            if (true == populated || null == inventoryItemPrefab || null == bodiesList || 0 == bodiesList.Count || null == headsList || 0 == headsList.Count) return;

            populated = true;
            // Initialize the bodies
            int itemsCount = bodiesList.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                BodyItemData itemData = bodiesList[i];
                // Create an "inventory item" for each variant
                for (int j = 0; j < itemData.GetVariantsCount(); j++)
                {
                    GameObject newItem = Instantiate(inventoryItemPrefab);
                    InventoryItem item = newItem.GetComponent<InventoryItem>();
                    inventoryItems.Add(item);
                    if (null == item) return;
                    item.InitializeItem(itemData.GetVariantName(j), null, null, itemData.ItemSprite, "", itemData.ItemID, ItemCategory.Body, itemData.ItemSet, j);
                    if (skinData.GetItemID(ItemCategory.Body) == itemData.ItemID && j == skinData.GetTextureIndex(ItemCategory.Body))
                    {
                        // This is the currently equipped variant!
                        item.Equip(equippedButtonSprite);
                        currentBody = item;
                    }
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
            itemsCount = headsList.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                HeadItemData itemData = headsList[i];
                if (itemData.HasVariants)
                {
                    VariantHeadItemData variantData = itemData as VariantHeadItemData;
                    // Create an "inventory item" for each variant
                    for (int j = 0; j < variantData.GetVariantsCount(); j++)
                    {
                        GameObject newItem = Instantiate(inventoryItemPrefab);
                        InventoryItem item = newItem.GetComponent<InventoryItem>();
                        inventoryItems.Add(item);
                        if (null == item) return;
                        item.InitializeItem(variantData.GetVariantName(j), null, null, variantData.ItemSprite, "", variantData.ItemID, ItemCategory.Head, itemData.ItemSet, j);
                        if (skinData.GetItemID(ItemCategory.Head) == variantData.ItemID && j == skinData.GetTextureIndex(ItemCategory.Head))
                        {
                            // This is the currently equipped variant
                            item.Equip(equippedButtonSprite);
                            currentHead = item;
                        }
                        item.MyButton.onClick.AddListener(OnInventoryItemButtonPressed);
                    }
                }
                else
                {
                    // Create 1 inventory item only
                    GameObject newItem = Instantiate(inventoryItemPrefab);
                    InventoryItem item = newItem.GetComponent<InventoryItem>();
                    inventoryItems.Add(item);
                    if (null == item) return;
                    item.InitializeItem(itemData.ItemName, null, null, itemData.ItemSprite, "", itemData.ItemID, ItemCategory.Head, itemData.ItemSet);
                    if (skinData.GetItemID(ItemCategory.Head) == itemData.ItemID)
                    {
                        // This is the currently equipped head
                        item.Equip(equippedButtonSprite);
                        currentHead = item;
                    }
                    item.MyButton.onClick.AddListener(OnInventoryItemButtonPressed);
                }
            }

            // Initialize the item sorter
            itemSorter.Initialize(inventoryItems);
            // Sort by rarity, the default sort method
            itemSorter.Sort(inventoryItems, SortMethod.Rarity);
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

        public override void OnInventoryItemButtonPressed()
        {
            // Get the currently selected inventory item
            InventoryItem selectedItem = EventSystem.current.currentSelectedGameObject.GetComponent<InventoryItem>();
            if (selectedItem != null)
            {
                if (selectedItem.ItemType == ItemCategory.Body)
                {
                    // Get the item corresponding to that button
                    for (int i = 0; i < bodiesList.Count; i++)
                    {
                        if (bodiesList[i].ItemID == selectedItem.ItemID)
                        {
                            skinData.SetItemID(ItemCategory.Body, bodiesList[i].ItemID);
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
                    for (int i = 0; i < headsList.Count; i++)
                    {
                        if (headsList[i].ItemID == selectedItem.ItemID)
                        {
                            skinData.SetItemID(ItemCategory.Head, headsList[i].ItemID);
                            if (headsList[i].HasVariants)
                                skinData.SetTextureIndex(ItemCategory.Head, selectedItem.VariantNumber);
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

            // Sync the player's skin data
            SaveManager.SaveData(SaveManager.charaSkinKey, skinData, SaveManager.relativeSkinDataPath);
            AccountManager.SyncCharaSkinData().WrapErrors();
        }
    }
}