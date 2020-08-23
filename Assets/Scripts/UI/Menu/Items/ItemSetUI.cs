using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Poomf.UI
{
    public class ItemSetUI : MonoBehaviour
    {
        [SerializeField] Text setNameText = null;
        [SerializeField] Text setPriceText = null;
        [SerializeField] Text setItemsCountText = null;
        [SerializeField] Button buyAllButton = null;
        [SerializeField] Transform contentParent = null;

        List<InventoryItem> itemsInSet = null;
        public string SetName { get { return setNameText.text; } }
        public ItemSet Set { get; private set; }
        public ItemCategory Category { get; private set; }

        public void Initialize(List<InventoryItem> items)
        {
            itemsInSet = items;
            Set = items[0].ItemSet;
            Category = items[0].ItemType;
            setNameText.text = Set.ToString();
            UpdateItemsCount();
            UpdateSetPrice();
        }

        public void AttachItem(InventoryItem item)
        {
            if (item.ItemSet != Set)
            {
                Debug.LogWarning("ItemSetUI -> AttachItem: Trying to attach an item with a different set.");
                return;
            }
            item.transform.SetParent(contentParent);
        }

        public void UpdateItemsCount()
        {
            int ownedItems = 0;
            // TODO: check the number of owned items
            /*
            for (int i = 0; i < itemsInSet.Count; i++)
            {
                if()
            } */

            setItemsCountText.text = ownedItems + "/" + itemsInSet.Count;
        }

        public void UpdateSetPrice()
        {
            int sum = 0;
            // Sum all the prices of all items
            for (int i = 0; i < itemsInSet.Count; i++)
            {
                // TODO: sum prices, check if coins or gems
            }

            setPriceText.text = sum.ToString();
        }
    }
}