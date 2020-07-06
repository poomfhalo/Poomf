using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//To Add New Item Types Here: 
//Add it to the enum, then assign CustomItem to the appropriate game object and assign the itemTypeField to the new enum type.
//Create a new field on all CharaSkinData Objects, that corrosponds to the new ItemType
public enum ItemType { Head, Outfit }

//New Concept: SkinItemData
//Each character will have a skin, which is made up of items, SkinItemData, will be responsible, for holding the data related
//to a single item, such as, what colors are of the item, or what textures.
[Serializable]
public class SkinItemData
{
    [Header("Manually Assigned")]//Can not be assigned in game
    public ItemType type = ItemType.Head;
    [Header("Assigned By UI")]//Can be assigned in game, or in inspector
    public int activeItemID = 0;
    public List<Color> colors = new List<Color> { Color.green };
    //Never Call, Setters/Getters of this class, from anywhere other than CharaSkinData
    public Color GetColor(int i) => colors[GetClamppedColorIndex(i)];
    public void SetColor(int i, Color c) => colors[GetClamppedColorIndex(i)] = c;
    private int GetClamppedColorIndex(int i) => i = Mathf.Clamp(i, 0, colors.Count - 1);
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

    public void SetItemID(ItemType outfit, int itemID) { GetItemData(outfit).activeItemID = itemID; Refresh(); }
    public void SetColor(ItemType ofItem, int index, Color c) { GetItemData(ofItem).SetColor(index, c); Refresh(); }
    public Color GetColor(ItemType ofItem, int index) => GetItemData(ofItem).GetColor(index);
    public int GetItemID(ItemType ofItem) => GetItemData(ofItem).activeItemID;
    SkinItemData GetItemData(ItemType itemType) => items.Single(i => i.type == itemType);
    private void Refresh() => onDataUpdated?.Invoke();
}