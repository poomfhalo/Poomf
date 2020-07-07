using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyBox;

namespace Poomf.UI
{
    public class MenuLockerColorOption : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [Header("RGB Sliders")]
        [SerializeField] private bool isRGBAdjustable = true;
        [SerializeField, ConditionalField("isRGBAdjustable")] private Slider redSlider = null;
        [SerializeField, ConditionalField("isRGBAdjustable")] private Slider greenSlider = null;
        [SerializeField, ConditionalField("isRGBAdjustable")] private Slider blueSlider = null;

        [SerializeField] CharaSkinData skinData = null;
        [SerializeField] ItemCategory itemToModify = ItemCategory.Head;

        private bool visible = false;

        public void Initialize(CharaSkinData skinData)
        {
            this.skinData = skinData;
            // Update the sliders values with the current equipment's colors
            if (isRGBAdjustable)
            {
                Color currentColor = skinData.GetColor(itemToModify, 0);
                redSlider.value = currentColor.r;
                greenSlider.value = currentColor.g;
                blueSlider.value = currentColor.b;
            }
        }
        public void Appear()
        {
            visible = !visible;
            animator.SetBool("appear", visible);
        }

        // Called when one of the Color buttons is pressed. Sets the current slot's color to the selected color.
        public void OnColorButtonSelected()
        {
            // Get the currently selected button's image and send its color to the equip slot
            Image buttonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (skinData.IsColorable(itemToModify))
                {
                    // The item type uses colors, modify their colors' values
                    skinData.SetColor(itemToModify, 0, buttonImage.color);
                    if (isRGBAdjustable)
                    {
                        // Update the RGB sliders to reflect the selected color's RGB values
                        redSlider.value = buttonImage.color.r;
                        greenSlider.value = buttonImage.color.g;
                        blueSlider.value = buttonImage.color.b;
                    }
                }
                else if (skinData.IsTextureCustomizable(itemToModify))
                {
                    // The item type uses textures rather than colors. Update its texture.
                    // Use the button's order in the hierarchy as an index to textures
                    skinData.SetTextureIndex(itemToModify, buttonImage.transform.GetSiblingIndex());
                }
            }
        }

        // Called when Sliders values change. Updates the corresponding color value in the equipment
        public void AdjustRed(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify, 0);
            Color newColor = new Color(value, currentColor.g, currentColor.b);
            skinData.SetColor(itemToModify, 0, newColor);
        }

        public void AdjustGreen(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify, 0);
            Color newColor = new Color(currentColor.r, value, currentColor.b);
            skinData.SetColor(itemToModify, 0, newColor);
        }

        public void AdjustBlue(float value)
        {
            Color currentColor = skinData.GetColor(itemToModify, 0);
            Color newColor = new Color(currentColor.r, currentColor.g, value);
            skinData.SetColor(itemToModify, 0, newColor);
        }
    }
}