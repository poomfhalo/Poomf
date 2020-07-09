using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

//To Add New Item Types Here: 
//Add it to the enum, then assign CustomItem to the appropriate game object and assign the itemTypeField to the new enum type.
//Create a new field on all CharaSkinData Objects, that corrosponds to the new ItemType
public enum ItemCategory { Head, Body, Eyes, Skin }


//New Concept: SkinItemData
//Each character will have a skin, which is made up of items, SkinItemData, will be responsible, for holding the data related
//to a single item, such as, what colors are of the item, or what textures.
[Serializable]
public class SkinItemData
{
    [Header("Manually Assigned")]//Can not be assigned in game
    public ItemCategory type = ItemCategory.Head;
    [Tooltip("Can this type be colored (using Colors not Textures) or not?")]
    public bool isColorable = false;
    [Tooltip("Does this type have several interchangeable textures?")]
    public bool isTextureCustomizable = false;

    [Header("Assigned By UI")]//Can be assigned in game, or in inspector
    public int activeItemID = 0;
    [ConditionalField("isColorable")] public List<Color> colors = new List<Color> { Color.green };
    // The currently active texture of Texture customizable items like outfits
    [ConditionalField("isTextureCustomizable")] public int currentTextureIndex = 0;
    //Never Call, Setters/Getters of this class, from anywhere other than CharaSkinData
    public Color GetColor(int i) => colors[GetClamppedColorIndex(i)];
    public void SetColor(int i, Color c) => colors[GetClamppedColorIndex(i)] = c;
    private int GetClamppedColorIndex(int i) => i = Mathf.Clamp(i, 0, colors.Count - 1);

    public int GetTextureIndex() => currentTextureIndex;
    public void SetTextureIndex(int index) => currentTextureIndex = index;
}
//New Concept: CharaSkinData:
//Characters will be customized, they'll be customized through items simply changing, so it makes sense
//to have one object, to hold the entirety of the SkinData.
//To Add new data, such as Textures, follow the same pattern, by creating a Set function in CharaSkinData and SkinItemData
[CreateAssetMenu(fileName = "CharaSkinData", menuName = "Dodgeball/CharaSkinData")]
public class CharaSkinData : ScriptableObject
{
    public Gender gender = Gender.FEMALE;
    public event Action onDataUpdated = null;
    public List<SkinItemData> items = new List<SkinItemData>();

    public void SetItemID(ItemCategory outfit, int itemID) { GetItemData(outfit).activeItemID = itemID; Refresh(); }
    public void SetColor(ItemCategory ofItem, int index, Color c)
    {
        if (!GetItemData(ofItem).isColorable)
        {
            Debug.LogWarning("Trying to color a/an " + GetItemData(ofItem).type.ToString() + ", which is non colorable!");
            return;
        }
        GetItemData(ofItem).SetColor(index, c);
        Refresh();
    }
    public Color GetColor(ItemCategory ofItem, int index) => GetItemData(ofItem).GetColor(index);

    public int GetTextureIndex(ItemCategory ofItem) => GetItemData(ofItem).GetTextureIndex();
    public void SetTextureIndex(ItemCategory ofItem, int index)
    {
        if (!GetItemData(ofItem).isTextureCustomizable)
        {
            Debug.LogWarning("Trying to change the texture of a/an " + GetItemData(ofItem).type.ToString() + ", which don't have textures!");
            return;
        }
        GetItemData(ofItem).SetTextureIndex(index);
        Refresh();
    }

    public int GetItemID(ItemCategory ofItem) => GetItemData(ofItem).activeItemID;
    SkinItemData GetItemData(ItemCategory itemType) => items.Single(i => i.type == itemType);
    public bool IsColorable(ItemCategory itemType) => GetItemData(itemType).isColorable;
    public bool IsTextureCustomizable(ItemCategory itemType) => GetItemData(itemType).isTextureCustomizable;
    private void Refresh() => onDataUpdated?.Invoke();
}