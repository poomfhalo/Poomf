using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poomf.Data;
using UnityEngine.AddressableAssets;

namespace Poomf.UI
{
    // Used with any menu that has an inventory of items (shop, locker, etc)
    public abstract class MenuItemInventory : MenuItemBase
    {
        [SerializeField] protected GameObject inventoryItemPrefab = null;
        [SerializeField] protected ItemSorter itemSorter = null;

        // TODO: central place for all item references
        protected List<HeadItemData> headsList = new List<HeadItemData>();
        protected List<BodyItemData> bodiesList = new List<BodyItemData>();

        protected List<InventoryItem> inventoryItems = new List<InventoryItem>();
        protected SortMethod currentSort = SortMethod.Rarity;

        bool itemsLoaded = false;

        protected override void Awake()
        {
            base.Awake();
            if (itemsLoaded)
                Populate();
            else
            {
                LoadItemsData();
                itemsLoaded = true;
            }
        }

        // Populate the inventory with items
        // TODO: Suitable implementation when Shop is working
        protected virtual void Populate()
        {

        }

        // Called when a button in the inventory is pressed
        // TODO: Suitable implementation when Shop is working
        public virtual void OnInventoryItemButtonPressed()
        {

        }

        public void OnSortButtonPressed()
        {
            // Use the next sorting method
            currentSort++;
            if ((int)currentSort >= itemSorter.SortMethodsCount)
            {
                // Use the first sorting method
                currentSort = 0;
            }

            itemSorter.Sort(inventoryItems, currentSort);
        }


        #region Addressables and their Delegates
        // Callback that's called each time a body data is loaded
        event System.Action<BodyItemData> onBodyLoaded;
        // Callback that's called each time a head data is loaded
        event System.Action<HeadItemData> onHeadLoaded;
        void LoadItemsData()
        {
            Debug.Log("POP" + gameObject.name);

            // Add events
            onBodyLoaded += OnBodyLoaded;
            onHeadLoaded += OnHeadLoaded;
            // Load the heads data
            Addressables.LoadAssetsAsync<HeadItemData>("Heads Data", onHeadLoaded);
            // Load the bodies data
            Addressables.LoadAssetsAsync<BodyItemData>("Bodies Data", onBodyLoaded).Completed += operation =>
            {
                // All data has been loaded, populate the locker
                Populate();
            };
        }
        void OnBodyLoaded(BodyItemData body)
        {
            bodiesList.Add(body);
        }
        void OnHeadLoaded(HeadItemData head)
        {
            headsList.Add(head);
        }
        #endregion
    }
}