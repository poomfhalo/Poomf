﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poomf.Data
{
    [CreateAssetMenu(fileName = "BodyItemData", menuName = "ScriptableObjects/Items/BodyItemData", order = 2)]
    public class BodyItemData : ItemDataBase
    {
        [Tooltip("Names appended to the original item's name that distinguishes between different variants.")]
        [SerializeField] string[] variantNamesExtensions = new string[3] { "A", "B", "C" };

        [Tooltip("The amount of coins added to each variant's price. 0 means it costs the same as its \"PriceCoins\"")]
        [SerializeField] int[] variantAdditionalCoinPrices = new int[3] { 0, 0, 0 };

        [Tooltip("The amount of gems added to each variant's price. 0 means it costs the same as its \"PriceGems\"")]
        [SerializeField] int[] variantAdditionalGemPrices = new int[3] { 0, 0, 0 };

        [SerializeField, HideInInspector] bool initialized = false;
        private void OnEnable()
        {
            if (!initialized)
            {
                ItemCategory = ItemCategory.Body;
                initialized = true;
            }
        }

        public string GetVariantName(int index)
        {
            return ItemName + " " + variantNamesExtensions[index];
        }

        public int GetVariantCoinPrice(int index)
        {
            return PriceCoins + variantAdditionalCoinPrices[index];
        }

        public int GetVariantGemPrice(int index)
        {
            return PriceGems + variantAdditionalGemPrices[index];
        }

        public int GetVariantsCount()
        {
            return variantNamesExtensions.Length;
        }
    }
}