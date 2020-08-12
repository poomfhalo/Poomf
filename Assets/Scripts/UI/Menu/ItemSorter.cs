using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

namespace Poomf.UI
{
    public enum SortMethod
    {
        Rarity,
        Set
    }
    public class ItemSorter : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] GameObject generalSortPanel = null;
        [SerializeField] Transform headsContentParent = null;
        [SerializeField] Transform bodiesContentParent = null;
        [SerializeField] Text currentSortText = null;
        [Header("Sort by Set")]
        [SerializeField] GameObject setSortPanel = null;
        [SerializeField] Transform setsContentParent = null;
        [SerializeField] GameObject itemSetPrefab = null;

        public List<ItemSetUI> ItemSets { get; private set; } = new List<ItemSetUI>();
        public int SortMethodsCount { get; private set; }

        public void Initialize(List<InventoryItem> items)
        {
            // Add a set gameobject for each item set available
            int count = Enum.GetNames(typeof(ItemSet)).Length;
            for (int i = 0; i < count; i++)
            {
                // Dont create a set for items without sets
                if ((ItemSet)i == ItemSet.NONE)
                    continue;
                List<InventoryItem> itemsInSet = items.FindAll(x => x.ItemSet == (ItemSet)i);
                if (itemsInSet.Count == 0)
                {
                    // No items belonging to this set, skip
                    continue;
                }
                GameObject set = Instantiate(itemSetPrefab, setsContentParent);
                ItemSetUI setUI = set.GetComponent<ItemSetUI>();
                ItemSets.Add(setUI);

                // Add the relevant items to that set
                setUI.Initialize(itemsInSet);
            }

            SortMethodsCount = Enum.GetNames(typeof(SortMethod)).Length;
        }

        public void Sort(List<InventoryItem> itemsToSort, SortMethod method)
        {
            if (method == SortMethod.Rarity)
                SortByRarity(itemsToSort);
            else if (method == SortMethod.Set)
                SortBySet(itemsToSort);
        }

        void SortByRarity(List<InventoryItem> itemsToSort)
        {
            HideAllPanels();
            generalSortPanel.SetActive(true);
            // TODO: sort by rarity
            for (int i = 0; i < itemsToSort.Count; i++)
            {
                if (itemsToSort[i].ItemType == ItemCategory.Body)
                {
                    itemsToSort[i].transform.SetParent(bodiesContentParent, false);
                }
                else if (itemsToSort[i].ItemType == ItemCategory.Head)
                {
                    itemsToSort[i].transform.SetParent(headsContentParent, false);
                }
            }

            currentSortText.text = "RARITY";
        }

        void SortBySet(List<InventoryItem> itemsToSort)
        {
            HideAllPanels();
            setSortPanel.SetActive(true);
            // Get a copy of the original list
            itemsToSort = itemsToSort.GetCopy();
            // Remove items without sets
            itemsToSort.RemoveAll(x => x.ItemSet == ItemSet.NONE);
            // For each set, put the items of the same set under it
            for (int i = 0; i < ItemSets.Count; i++)
            {
                for (int j = 0; j < itemsToSort.Count; j++)
                {
                    if (itemsToSort[j].ItemSet == ItemSets[i].Set || ItemSets[i].SetName == string.Empty)
                    {
                        ItemSets[i].AttachItem(itemsToSort[j]);
                    }
                }
            }

            currentSortText.text = "SETS";
        }

        void HideAllPanels()
        {
            generalSortPanel.SetActive(false);
            setSortPanel.SetActive(false);
        }
    }
}