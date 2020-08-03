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
    [Header("Assigned By UI")]//Can be assigned in game, or in inspector
    public int activeItemID = 0;
    [ConditionalField(nameof(type), false, ItemCategory.Head)] public Color currentColor = Color.white;
    // Used with items that use preset colors rather than arbitrary ones like Eyes
    [ConditionalField(nameof(type), false, ItemCategory.Eyes, ItemCategory.Skin)] public int currentColorIndex = 0;
    // The currently active texture of Texture customizable items like outfits
    [ConditionalField(nameof(type), false, ItemCategory.Eyes, ItemCategory.Body, ItemCategory.Head)] public int currentTextureIndex = 0;
    //Never Call, Setters/Getters of this class, from anywhere other than CharaSkinData
    public Color GetColor() => currentColor;
    public void SetColor(Color c) => currentColor = c;

    public int GetColorIndex() => currentColorIndex;
    public void SetColorIndex(int index) => currentColorIndex = index;

    public int GetTextureIndex() => currentTextureIndex;
    public void SetTextureIndex(int index) => currentTextureIndex = index;
}
//New Concept: CharaSkinData:
//Characters will be customized, they'll be customized through items simply changing, so it makes sense
//to have one object, to hold the entirety of the SkinData.
//To Add new data, such as Textures, follow the same pattern, by creating a Set function in CharaSkinData and SkinItemData
[CreateAssetMenu(fileName = "CharaSkinData", menuName = "Dodgeball/CharaSkinData")]
public class CharaSkinData : PersistantSO
{
    public Gender gender = Gender.FEMALE;
    public event Action onDataUpdated = null;
    public List<SkinItemData> items = new List<SkinItemData>();

    public void SetItemID(ItemCategory outfit, int itemID) { GetItemData(outfit).activeItemID = itemID; Refresh(); }
    public void SetColor(ItemCategory ofItem, Color c)
    {
        GetItemData(ofItem).SetColor(c);
        Refresh();
    }
    public Color GetColor(ItemCategory ofItem) => GetItemData(ofItem).GetColor();

    public void SetColorIndex(ItemCategory ofItem, int index)
    {
        GetItemData(ofItem).SetColorIndex(index);
        Refresh();
    }
    public int GetColorIndex(ItemCategory ofItem) => GetItemData(ofItem).GetColorIndex();

    public int GetTextureIndex(ItemCategory ofItem) => GetItemData(ofItem).GetTextureIndex();
    public void SetTextureIndex(ItemCategory ofItem, int index)
    {
        GetItemData(ofItem).SetTextureIndex(index);
        Refresh();
    }

    public int GetItemID(ItemCategory ofItem) => GetItemData(ofItem).activeItemID;
    SkinItemData GetItemData(ItemCategory itemType)
    {
        Debug.LogWarning("Tryign To find " + itemType);
        string s = "";
        foreach (var item in items)
        {
            s = s + item.type + " , ";
        }
        Debug.LogWarning(s);
        return items.Single(i => i.type == itemType);
    }
    private void Refresh() => onDataUpdated?.Invoke();

    public override string ToString()
    {
        string s = "(";
        foreach (var id in items)
        {
            s = s + id.activeItemID + ",";
        }
        s = s + ")";
        return name + " IDS: " + s; 
    }
}

//To turn the CharaSkinData, to plain data that can be transfered across the network.
[Serializable]
public class CharaSkinDataPlain
{
    public bool IsEmpty => types.Length == 0;
    public int[] types = new int[0];
    public int[] ids = new int[0];

    public float[] reds = new float[0];
    public float[] greens = new float[0];
    public float[] blues = new float[0];

    public int[] colorIndicies = new int[0];
    public int[] texesIndicies = new int[0];

    public CharaSkinDataPlain() { }

    public CharaSkinDataPlain(CharaSkinData skinData)
    {
        if (skinData == null)
            return;

        int itemsCount = skinData.items.Count;

        types = new int[itemsCount];
        ids = new int[itemsCount];
        colorIndicies = new int[itemsCount];
        texesIndicies = new int[itemsCount];

        reds = new float[itemsCount];
        greens = new float[itemsCount];
        blues = new float[itemsCount]; 

        for (int i = 0; i < itemsCount; i++)
        {
            SkinItemData item = skinData.items[i];
            types[i] = (int)item.type;
            ids[i] = item.activeItemID;

            reds[i] = item.currentColor.r;
            greens[i] = item.currentColor.g;
            blues[i] = item.currentColor.b;

            colorIndicies[i] = item.currentColorIndex;
            texesIndicies[i] = item.currentTextureIndex;
        }
    }
    public CharaSkinDataPlain(int[] types, int[] ids, float[] reds, float[] greens, float[] blues, int[] colorIndicies, int[] texesIndicies)
    {
        this.types = types;
        this.ids = ids;
        this.reds = reds;
        this.greens = greens;
        this.blues = blues;
        this.colorIndicies = colorIndicies;
        this.texesIndicies = texesIndicies;
    }

    public CharaSkinData CreateSkinData()
    {
        int itemsCount = types.Length;
        CharaSkinData skinData = ScriptableObject.CreateInstance<CharaSkinData>();
        skinData.items = new List<SkinItemData>();

        for (int i = 0; i < itemsCount; i++)
        {
            SkinItemData item = new SkinItemData();
            item.type = (ItemCategory)types[i];
            item.activeItemID = ids[i];

            Color c = new Color(reds[i], greens[i], blues[i]);
            item.SetColor(c);

            item.SetColorIndex(colorIndicies[i]);
            item.SetTextureIndex(texesIndicies[i]);
            skinData.items.Add(item);
        }
        return skinData;
    }
    public override string ToString()
    {
        string s = "(";
        foreach (var id in ids)
        {
            s = s + id + ",";
        }
        s = s + ")";
        return " IDS: " + s;
    }
}