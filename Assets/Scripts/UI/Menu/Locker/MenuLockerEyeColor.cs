using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuLockerEyeColor : MenuLockerColorOption
    {
        [Header("Eye Types")]
        [SerializeField] Text typeNameText = null;
        [SerializeField] string[] eyeTypeNames = null;

        int eyeTypeCount = 0;
        // The currently selected type
        int currentType = 0;

        private void Awake()
        {
            itemToModify = ItemCategory.Eyes;
            eyeTypeCount = eyeTypeNames.Length;

            currentType = skinData.GetTextureIndex(itemToModify);
            SetTypeName(currentType);
        }

        // Updates the type name text to the selected index.
        void SetTypeName(int index)
        {
            if (index >= eyeTypeCount)
            {
                Debug.LogError("MenuLockerEyeColor -> SetTypeName: Supplied index is out of bound!");
                return;
            }
            typeNameText.text = eyeTypeNames[index];
        }

        public override void OnColorButtonSelected()
        {
            // Get the currently selected button's image and send its color to the equip slot
            Image buttonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Eyes use preset color indices. Use the button's order in the hierarchy as an index
                skinData.SetColorIndex(itemToModify, buttonImage.transform.GetSiblingIndex());
            }
        }

        public void OnNextTypeButtonPressed()
        {
            if (currentType < eyeTypeCount - 1)
            {
                // Haven't reached the last item yet. Switch to the next one.
                currentType++;
                skinData.SetTextureIndex(itemToModify, currentType);
                SetTypeName(currentType);
            }
        }

        public void OnPreviousTypeButtonPressed()
        {
            if (currentType > 0)
            {
                // Haven't reached the first item yet. Switch to the previous one. 
                currentType--;
                skinData.SetTextureIndex(itemToModify, currentType);
                SetTypeName(currentType);
            }
        }
    }
}
