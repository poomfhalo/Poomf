using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizablePlayer : MonoBehaviour
{
    [SerializeField] Gender gender = Gender.MALE;
    [Header("Custom Objects' Parents")]
    [SerializeField] Transform customHeadsParent = null;
    [SerializeField] Transform customOutfitsParent = null;
    [Header("Equipment Slots")]
    [SerializeField] CustomEquipmentSlot headSlot = null;
    [SerializeField] CustomEquipmentSlot outfitSlot = null;

    // Lists that contain all custom items
    List<CustomItem> customHeads = new List<CustomItem>();
    List<CustomItem> customOutfits = new List<CustomItem>();

    #region Setters/Getters
    public CustomEquipmentSlot HeadSlot { get { return headSlot; } private set { headSlot = value; } }
    public CustomEquipmentSlot OutfitSlot { get { return outfitSlot; } private set { outfitSlot = value; } }
    #endregion

    private void Start()
    {
        Initialize();
    }


    // Initializes the custom items lists, all custom items and equipment slots
    void Initialize()
    {
        foreach (Transform child in customHeadsParent)
        {
            if (child.tag == "Head")
            {
                // Make sure the object is inactive
                child.gameObject.SetActive(false);
                CustomItem item = child.GetComponent<CustomItem>();
                // Add it to the list
                customHeads.Add(item);
                // Initialize it
                item.Initialize();
            }
        }

        foreach (Transform child in customOutfitsParent)
        {
            if (child.tag == "Outfit")
            {
                // Make sure the object is inactive
                child.gameObject.SetActive(false);
                CustomItem item = child.GetComponent<CustomItem>();
                // Add it to the list
                customOutfits.Add(item);
                // Initialize it
                item.Initialize();
            }
        }

        // Initialize the slots
        headSlot.Initialize(customHeads[headSlot.CurrentItemIndex]);
        outfitSlot.Initialize(customOutfits[outfitSlot.CurrentItemIndex]);
    }


}
