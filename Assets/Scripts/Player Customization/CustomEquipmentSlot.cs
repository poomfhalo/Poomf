using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomEquipmentSlot", menuName = "ScriptableObjects/Customization/CustomEquipmentSlot", order = 1)]
public class CustomEquipmentSlot : ScriptableObject
{
    // The index of the currently equipped item in the hierarchy.
    [SerializeField, HideInInspector] int currentItemIndex = 0;

    // Is there an item equipped already?
    [SerializeField, HideInInspector] bool isOccupied = false;

    // The currently equipped game object
    CustomItem currentEquip = null;

    #region Setters/Getters
    public int CurrentItemIndex { get { return currentItemIndex; } set { currentItemIndex = value; } }
    public bool IsOccupied { get { return isOccupied; } private set { isOccupied = value; } }
    public CustomItem CurrentEquip { get { return currentEquip; } private set { currentEquip = value; } }
    #endregion

    public void Initialize(CustomItem equipment)
    {
        if (currentEquip == null)
        {
            // First run! 
            Equip(equipment);
        }
        else if (currentEquip != null && isOccupied)
        {
            // Set the correct reference to the current equipped game object and enable it
            currentEquip = equipment;
            currentEquip.gameObject.SetActive(true);
        }
    }
    public void Equip(CustomItem equipment)
    {
        if (IsOccupied && currentEquip != null)
        {
            // Disable the currently equipped object. 
            currentEquip.gameObject.SetActive(false);
        }
        equipment.gameObject.SetActive(true);
        currentEquip = equipment;
        IsOccupied = true;
        // Get the current equipment's position in the hierarchy
        CurrentItemIndex = currentEquip.transform.GetSiblingIndex();
    }

    public void Unequip()
    {
        // Disable the equipped object
        if (IsOccupied)
        {
            currentEquip.gameObject.SetActive(false);
            CurrentItemIndex = -1;
            IsOccupied = false;
        }
    }
}
