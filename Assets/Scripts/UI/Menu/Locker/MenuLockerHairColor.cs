using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuLockerHairColor : MenuLockerColorOption
    {
        [SerializeField] private Slider redSlider = null;
        [SerializeField] private Slider greenSlider = null;
        [SerializeField] private Slider blueSlider = null;

        private void Awake()
        {
            itemToModify = ItemCategory.Head;
        }

        public override void Initialize(CharaSkinData skinData)
        {
            base.Initialize(skinData);
            // Update the RGB sliders' values to the current color's values
            Color currentColor = skinData.GetColor(itemToModify);
            redSlider.value = currentColor.r;
            greenSlider.value = currentColor.g;
            blueSlider.value = currentColor.b;
        }

        // Called when one of the Color buttons is pressed. Sets the current slot's color to the selected color.
        public override void OnColorButtonSelected()
        {
            // Get the currently selected button's image and send its color to the equip slot
            Image buttonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Hair uses colors, set its color to the selected button's color
                skinData.SetColor(itemToModify, buttonImage.color);
                // Update the RGB sliders to reflect the selected color's RGB values
                redSlider.value = buttonImage.color.r;
                greenSlider.value = buttonImage.color.g;
                blueSlider.value = buttonImage.color.b;
            }
        }
        // Called when Sliders values change. Updates the corresponding color value in the equipment
        public void AdjustRed(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify);
            Color newColor = new Color(value, currentColor.g, currentColor.b);
            skinData.SetColor(itemToModify, newColor);
        }

        public void AdjustGreen(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify);
            Color newColor = new Color(currentColor.r, value, currentColor.b);
            skinData.SetColor(itemToModify, newColor);
        }

        public void AdjustBlue(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify);
            Color newColor = new Color(currentColor.r, currentColor.g, value);
            skinData.SetColor(itemToModify, newColor);
        }
    }
}