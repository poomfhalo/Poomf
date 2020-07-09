#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Poomf.Data
{
    public class ItemsImporter : AssetPostprocessor
    {
        private static bool debug = false;
        private delegate void UniqueIDDelegate(ItemsUniqueIDManager i_uniqueIDManager, ItemDataBase i_itemData);
        private const string ItemsUniqueIDManagerPath = "Assets/Scripts/Data/ScriptableObject_Instances/ItemsUniqueIDManager.asset";

        private enum DebugLevel
        {
            LOG = 0,
            WARNING = 1,
            ERROR = 2
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                debugLog("Reimported Asset: " + str);
                manageItemID(setUniqueID, str);
            }

            foreach (string str in deletedAssets)
            {
                manageItemID(removeUniqueID, str);
            }
        }

        private static void manageItemID(UniqueIDDelegate i_uniqueIDDelegate, string i_path)
        {
            if (i_path.Contains("Data/ScriptableObject_Instances/Test_Instances") || i_path.Contains("Data/ScriptableObject_Instances/Items"))
            {
                ItemDataBase itemData = AssetDatabase.LoadAssetAtPath<ItemDataBase>(i_path);

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
                    debugLog("Data is null.", DebugLevel.WARNING);
                }
            }
        }

        private static void setUniqueID(ItemsUniqueIDManager i_uniqueIDManager, ItemDataBase i_itemData)
        {
            int uniqueID = i_uniqueIDManager.GetUniqueID();

            if (false == i_itemData.SetItemID(uniqueID, debug))
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

        private static void removeUniqueID(ItemsUniqueIDManager i_uniqueIDManager, ItemDataBase i_itemData)
        {
            i_uniqueIDManager.RemoveUniqueID(i_itemData.ItemID);

            EditorUtility.SetDirty(i_uniqueIDManager);
            AssetDatabase.SaveAssets();
        }

        private static void debugLog(string i_log, DebugLevel i_debugLevel = DebugLevel.LOG)
        {
            if (false == debug) return;

            switch (i_debugLevel)
            {
                case DebugLevel.LOG:
                    Debug.Log(i_log);
                    break;
                case DebugLevel.WARNING:
                    Debug.LogWarning(i_log);
                    break;
                case DebugLevel.ERROR:
                    Debug.LogError(i_log);
                    break;
            }
        }
    }
}

#endif