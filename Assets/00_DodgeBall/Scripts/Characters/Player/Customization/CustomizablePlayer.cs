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
    [SerializeField] List<CustomItem> allEyes = new List<CustomItem>();
    [SerializeField] CustomItem playerSkinTone = null;

    void Start()
    {
        // Initializes the custom items lists, all custom items and equipment slots
        transform.GetComponentsInChildren<CustomItem>(true).ToList().ForEach(c =>
        {
            c.gameObject.SetActive(false);
            if (c.itemType == ItemCategory.Head)
                allHeads.Add(c);
            else if (c.itemType == ItemCategory.Outfit)
                allOutfits.Add(c);
            else if (c.itemType == ItemCategory.Eyes)
                allEyes.Add(c);
            else if (c.itemType == ItemCategory.Skin)
                playerSkinTone = c;
            c.Initialize();
        });
        skinData.onDataUpdated += RefreshCharaVisuals;
        RefreshCharaVisuals();
    }

    private void RefreshCharaVisuals()
    {
        CustomItem activeHead = allHeads.Single(h => h.ItemID == skinData.GetItemID(ItemCategory.Head));
        allHeads.ForEach(h => h.gameObject.SetActive(false));

        CustomItem activeOutFit = allOutfits.Single(h => h.ItemID == skinData.GetItemID(ItemCategory.Outfit));
        allOutfits.ForEach(o => o.gameObject.SetActive(false));

        CustomItem activeEyes = allEyes.Single(h => h.ItemID == skinData.GetItemID(ItemCategory.Eyes));
        allEyes.ForEach(o => o.gameObject.SetActive(false));

        activeOutFit.gameObject.SetActive(true);
        activeHead.gameObject.SetActive(true);
        activeEyes.gameObject.SetActive(true);
        playerSkinTone.gameObject.SetActive(true);

        RefreshItemVisuals(activeHead);
        RefreshItemVisuals(activeOutFit);
        RefreshItemVisuals(activeEyes);
        RefreshItemVisuals(playerSkinTone);
    }

    // Updates an item's visuals taking into account if they are colorable/texture-customizable or not
    private void RefreshItemVisuals(CustomItem item)
    {
        if (skinData.IsColorable(item.itemType))
            item.SetColor(skinData.GetColor(item.itemType, 0), 0);
        if (skinData.IsTextureCustomizable(item.itemType))
            item.SetTexture(skinData.GetTextureIndex(item.itemType));
    }
}