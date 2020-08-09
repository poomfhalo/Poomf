using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;
using Poomf.Data;

public class CustomizablePlayer : MonoBehaviour
{
    public CharaSkinData GetSkinData => skinData;
    [SerializeField] CharaSkinData skinData = null;

    // Lists that contain all custom items
    [Header("Read Only")]
    [SerializeField, ReadOnly] List<CustomHead> allHeads = new List<CustomHead>();
    [SerializeField, ReadOnly] List<CustomBody> allOutfits = new List<CustomBody>();
    [SerializeField, ReadOnly] CustomEyes playerEyes = null;
    [SerializeField, ReadOnly] CustomSkin playerSkinTone = null;
    [SerializeField] CharaSkinDataPlain plain = null;


    void Awake()
    {
        // Initializes the custom items lists, all custom items and equipment slots
        transform.GetComponentsInChildren<CustomItemBase>(true).ToList().ForEach(c =>
        {
            c.Initialize();
            c.gameObject.SetActive(false);
            if (c.itemType == ItemCategory.Head)
                allHeads.Add(c as CustomHead);
            else if (c.itemType == ItemCategory.Body)
                allOutfits.Add(c as CustomBody);
            else if (c.itemType == ItemCategory.Eyes)
                playerEyes = c as CustomEyes;
            else if (c.itemType == ItemCategory.Skin)
                playerSkinTone = c as CustomSkin;
        });
        skinData.onDataUpdated += RefreshCharaVisuals;
    }
    void Start()
    {
        skinData = SaveManager.GetData(SaveManager.charaSkinKey, skinData, SaveManager.relativeSkinDataPath);
        RefreshCharaVisuals();
    }
    void OnDestroy()
    {
        skinData.onDataUpdated -= RefreshCharaVisuals;
    }

    private void RefreshCharaVisuals()
    {
        //SaveManager.SaveData(SaveManager.charaSkinKey, skinData, SaveManager.relativeSkinDataPath);
        //AccountManager.SyncCharaSkinData().WrapErrors();
        //StartCoroutine(AccountManager.cloud.Sync(SaveManager.relativeSkinDataPath, AccountManager.Username));

        CustomHead activeHead = allHeads.Single(h => h.ItemID == skinData.GetItemID(ItemCategory.Head));
        allHeads.ForEach(h => h.gameObject.SetActive(false));

        CustomBody activeOutFit = allOutfits.Single(h => h.ItemID == skinData.GetItemID(ItemCategory.Body));
        allOutfits.ForEach(o => o.gameObject.SetActive(false));

        activeOutFit.gameObject.SetActive(true);
        activeHead.gameObject.SetActive(true);
        playerEyes.gameObject.SetActive(true);
        playerSkinTone.gameObject.SetActive(true);

        RefreshItemVisuals(activeHead);
        RefreshItemVisuals(activeOutFit);
        RefreshItemVisuals(playerEyes);
        RefreshItemVisuals(playerSkinTone);
        plain = new CharaSkinDataPlain(skinData);
    }

    // Updates an item's visuals taking into account if they are colorable/texture-customizable or not
    private void RefreshItemVisuals(CustomItemBase item)
    {
        // Call the 3 methods. Items that don't use any of them will simply not do anything as the method would be empty
        item.SetColor(skinData.GetColor(item.itemType));
        item.SetColor(skinData.GetColorIndex(item.itemType));
        item.SetTexture(skinData.GetTextureIndex(item.itemType));
    }

    public void SetNewSkinData(CharaSkinData skinData)
    {
        this.skinData = skinData;
        RefreshCharaVisuals();
    }
}