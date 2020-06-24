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
        [SerializeField] private Image itemImage = null;

        public void InitializeItem(string i_itemName, int? i_itemCoinsPrice, int? i_itemGemsPrice, Sprite itemSprite)
        {
            if (null != itemName)
                itemName.text = i_itemName;

            setupPrice(itemPriceCoins, i_itemCoinsPrice);
            setupPrice(itemPriceGems, i_itemGemsPrice);

            if (null != itemImage)
                itemImage.sprite = itemSprite;
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
    }
}