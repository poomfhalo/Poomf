#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Poomf.Data
{
    public class ItemsImporter : AssetPostprocessor
    {
        private delegate void UniqueIDDelegate(ItemsUniqueIDManager i_uniqueIDManager, ItemData i_itemData);
        private const string ItemsUniqueIDManagerPath = "Assets/Scripts/Data/ScriptableObject_Instances/ItemsUniqueIDManager.asset";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                Debug.Log("Reimported Asset: " + str);
                manageItemID(setUniqueID, str);
            }

            foreach (string str in deletedAssets)
            {
                manageItemID(removeUniqueID, str);
            }
        }

        private static void manageItemID(UniqueIDDelegate i_uniqueIDDelegate, string i_path)
        {
            if (i_path.Contains("Data/ScriptableObject_Instances/Test_Instances"))
            {
                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(i_path);

                if (null != itemData)
                {
                    ItemsUniqueIDManager uniqueIDManager = AssetDatabase.LoadAssetAtPath<ItemsUniqueIDManager>(ItemsUniqueIDManagerPath);

                    if (null != uniqueIDManager)
                    {
                        i_uniqueIDDelegate(uniqueIDManager, itemData);
                    }
                }
                else
                {
                    Debug.LogWarning("Data is null.");
                }
            }
        }

        private static void setUniqueID(ItemsUniqueIDManager i_uniqueIDManager, ItemData i_itemData)
        {
            int uniqueID = i_uniqueIDManager.GetUniqueID();

            if (false == i_itemData.SetItemID(uniqueID))
            {
                i_uniqueIDManager.RemoveUniqueID(uniqueID);
            }
            else
            {
                EditorUtility.SetDirty(i_itemData);
                EditorUtility.SetDirty(i_uniqueIDManager);
                AssetDatabase.SaveAssets();
            }
        }

        private static void removeUniqueID(ItemsUniqueIDManager i_uniqueIDManager, ItemData i_itemData)
        {
            i_uniqueIDManager.RemoveUniqueID(i_itemData.ItemID);

            EditorUtility.SetDirty(i_uniqueIDManager);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif