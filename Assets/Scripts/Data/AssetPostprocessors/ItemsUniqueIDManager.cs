using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemsUniqueIDManager", menuName = "ScriptableObjects/ItemsUniqueIDManager", order = 1)]
public class ItemsUniqueIDManager : ScriptableObject
{
    [HideInInspector] [SerializeField] private List<int> uniqueIDs = null;

    public int GetUniqueID()
    {
        initializeIDsList();

        Debug.Log("ItemsUniqueIDManager::GetUniqueID -> An item was created; generating unique ID...");

        if (0 == uniqueIDs.Count)
        {
            uniqueIDs.Add(0);
            return uniqueIDs[uniqueIDs.Count - 1];
        }
        else
        {
            uniqueIDs.Add(uniqueIDs[uniqueIDs.Count - 1] + 1);
            return uniqueIDs[uniqueIDs.Count - 1];
        }
    }

    public void RemoveUniqueID(int i_ID)
    {
        if (null == uniqueIDs) return;

        Debug.Log("ItemsUniqueIDManager::RemoveUniqueID -> An item was deleted; removing the unique ID...");

        if (true == uniqueIDs.Remove(i_ID))
        {
            Debug.Log("ItemsUniqueIDManager::RemoveUniqueID -> Removing unique ID successful.");
        }
        else
        {
            Debug.LogWarning("ItemsUniqueIDManager::RemoveUniqueID -> Removing unique ID failed, ID not found.");
        }
    }

    public void ResetAllIDs()
    {
        Debug.LogWarning("ItemsUniqueIDManager::ResetAllIDs -> Resetting all IDs...");
        uniqueIDs = null;
    }

    private void initializeIDsList()
    {
        if (null == uniqueIDs) uniqueIDs = new List<int>();
    }
}