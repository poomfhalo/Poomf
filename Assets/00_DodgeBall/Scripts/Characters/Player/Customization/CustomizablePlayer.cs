using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizablePlayer : MonoBehaviour
{
    public CharaSkinData GetSkinData => skinData;
    [SerializeField] CharaSkinData skinData = null; 

    // Lists that contain all custom items
    [Header("Read Only")]
    [SerializeField] List<CustomItem> allHeads = new List<CustomItem>();
    [SerializeField] List<CustomItem> allOutfits = new List<CustomItem>();

    void Start()
    {
        // Initializes the custom items lists, all custom items and equipment slots
        transform.GetComponentsInChildren<CustomItem>(true).ToList().ForEach(c => {
            c.gameObject.SetActive(false);
            if (c.itemType == ItemType.Head)
                allHeads.Add(c);
            else if (c.itemType == ItemType.Outfit)
                allOutfits.Add(c);
            c.Initialize();
        });
        skinData.onDataUpdated += RefreshCharaVisuals;
        RefreshCharaVisuals();
        //headSlot.Initialize(allHeads[headSlot.CurrentItemIndex]);
        //outfitSlot.Initialize(allOutfits[outfitSlot.CurrentItemIndex]);
    }
    public void EquipHead(int itemID)
    {
        // Get the item to equip from the heads list
        CustomItem itemToEquip = allHeads.Single(x => x.ItemID == itemID);
        // TODO: check gender
        skinData.SetItemID(ItemType.Head,itemID);
    }
    public void EquipOutfit(int itemID)
    {
        // Get the item to equip from the heads list
        CustomItem itemToEquip = allOutfits.Single(x => x.ItemID == itemID);
        // TODO: check gender
        skinData.SetItemID(ItemType.Outfit, itemID);
    }

    private void RefreshCharaVisuals()
    {
        //TODO: do change to Single() instead of Find(), after all IDS are unique.
        CustomItem activeHead= allHeads.Find(h => h.ItemID == skinData.GetItemID(ItemType.Head));
        allHeads.ForEach(h => h.gameObject.SetActive(false));
        //TODO: do change to Single() instead of Find(), after all IDS are unique.
        CustomItem activeOutFit = allOutfits.Find(h => h.ItemID == skinData.GetItemID(ItemType.Outfit));
        allOutfits.ForEach(o => o.gameObject.SetActive(false));

        activeOutFit.gameObject.SetActive(true);
        activeHead.gameObject.SetActive(true);

        activeHead.SetColor(skinData.GetColor(ItemType.Head, 0), 0);
        activeOutFit.SetColor(skinData.GetColor(ItemType.Outfit, 0), 0);
    }
}