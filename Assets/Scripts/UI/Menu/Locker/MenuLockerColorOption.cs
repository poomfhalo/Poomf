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

        // The slot that this menu is controlling its colors
        //public CustomEquipmentSlot EquipSlot;
        [SerializeField] CharaSkinData skinData = null;
        [SerializeField] ItemType itemToModify = ItemType.Head;

        private bool visible = false;

        //private void Start()
        public void Initialize(CharaSkinData skinData)
        {
            this.skinData = skinData;
            // Update the sliders values with the current equipment's colors
            //if (EquipSlot.IsOccupied && isRGBAdjustable)
            if(isRGBAdjustable)
            {
                //Color currentColor = EquipSlot.CurrentEquip.GetColor(0);
                Color currentColor = skinData.GetColor(itemToModify,0);
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
            if(buttonImage != null)
            //if (buttonImage != null && EquipSlot.IsOccupied)
            {
                skinData.SetColor(itemToModify, 0, buttonImage.color);
                //EquipSlot.CurrentEquip.SetColor(buttonImage.color, 0);
                if (isRGBAdjustable)
                {
                    // Update the RGB sliders to reflect the selected color's RGB values
                    redSlider.value = buttonImage.color.r;
                    greenSlider.value = buttonImage.color.g;
                    blueSlider.value = buttonImage.color.b;
                }
            }
        }

        // Called when Sliders values change. Updates the corresponding color value in the equipment
        public void AdjustRed(float value)
        {
            //NOTE: IsOccupied is no longer needed, as the SkinData, does not rely on CustomItem, it just holds the data.
            //if (EquipSlot.IsOccupied)
            //{
            //    Color currentColor = EquipSlot.CurrentEquip.GetColor(0);
            //    EquipSlot.CurrentEquip.SetColor(new Color(value, currentColor.g, currentColor.b), 0);
            //}
            Color currentColor = skinData.GetColor(itemToModify,0);
            Color newColor = new Color(value, currentColor.g, currentColor.b);
            skinData.SetColor(itemToModify, 0, newColor);
        }

        public void AdjustGreen(float value)
        {
            //if (EquipSlot.IsOccupied)
            //{
            //    Color currentColor = EquipSlot.CurrentEquip.GetColor(0);
            //    EquipSlot.CurrentEquip.SetColor(new Color(currentColor.r, value, currentColor.b), 0);
            //}
            Color currentColor = skinData.GetColor(itemToModify, 0);
            Color newColor = new Color(currentColor.r, value, currentColor.b);
            skinData.SetColor(itemToModify, 0, newColor);
        }

        public void AdjustBlue(float value)
        {
            //if (EquipSlot.IsOccupied)
            //{
            //    Color currentColor = EquipSlot.CurrentEquip.GetColor(0);
            //    EquipSlot.CurrentEquip.SetColor(new Color(currentColor.r, currentColor.g, value), 0);
            //}
            Color currentColor = skinData.GetColor(itemToModify, 0);
            Color newColor = new Color(currentColor.r, currentColor.g, value);
            skinData.SetColor(itemToModify, 0, newColor);
        }
    }
}