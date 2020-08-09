using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuLockerSkinColor : MenuLockerColorOption
    {
        private void Awake()
        {
            itemToModify = ItemCategory.Skin;
        }

        public override void OnColorButtonSelected()
        {
            // Get the currently selected button's image and send its color to the equip slot
            Image buttonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Skin uses preset color indices. Use the button's order in the hierarchy as an index
                skinData.SetColorIndex(itemToModify, buttonImage.transform.GetSiblingIndex());
                SyncData();
            }
        }
    }
}