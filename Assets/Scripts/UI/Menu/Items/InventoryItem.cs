﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poomf.UI
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Text itemName = null;
        [SerializeField] private Text itemPriceCoins = null;
        [SerializeField] private Text itemPriceGems = null;
        [SerializeField] private Text itemStatus = null;
        [SerializeField] private Image itemImage = null;

        public int ItemID { get; private set; }
        public ItemCategory ItemType { get; private set; }
        // Used with items that have variants like outfits.
        public int VariantNumber { get; private set; } = -1;
        public Button MyButton { get; private set; }
        public Image MyButtonImage { get; private set; }

        public void InitializeItem(string i_itemName, int? i_itemCoinsPrice, int? i_itemGemsPrice, Sprite itemSprite, string i_itemStatus, int itemID, ItemCategory itemType, int variantNumber = -1)
        {
            if (null != itemName)
                itemName.text = i_itemName;

            setupPrice(itemPriceCoins, i_itemCoinsPrice);
            setupPrice(itemPriceGems, i_itemGemsPrice);
            setupStatus(i_itemStatus);

            if (null != itemImage)
                itemImage.sprite = itemSprite;

            ItemID = itemID;
            ItemType = itemType;
            VariantNumber = variantNumber;
            MyButton = GetComponent<Button>();
            MyButtonImage = GetComponent<Image>();
        }

        public void Equip(Sprite equippedButtonSprite)
        {
            setupStatus("Equipped");
            MyButtonImage.sprite = equippedButtonSprite;
        }

        public void Unequip(Sprite defaultButtonSprite)
        {
            setupStatus("");
            MyButtonImage.sprite = defaultButtonSprite;
        }

        private void setupPrice(Text i_priceText, int? i_value)
        {
            if (null != i_priceText)
            {
                if (null != i_value)
                {
                    i_priceText.text = i_value.Value.ToString();
                    i_priceText.gameObject.SetActive(true);
                }
                else
                {
                    i_priceText.gameObject.SetActive(false);
                }
            }
        }

        private void setupStatus(string i_itemStatus)
        {
            if (null == itemStatus) return;

            itemStatus.text = i_itemStatus;
            itemStatus.gameObject.SetActive(!string.IsNullOrEmpty(i_itemStatus));
        }
    }
}