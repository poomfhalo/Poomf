using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameExtentions
{
    /// <summary>
    /// Make the specified objsGroup and index.
    /// </summary>
    /// <returns>The make.</returns>
    /// <param name="objsGroup">Objects group.</param>
    /// <param name="index">-1 means we select one randomly</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T Make<T>(T[] objsGroup,int index =-1) where T: UnityEngine.Object
    {
        if (objsGroup.Length == 0)
            return null;
        if (objsGroup.Length == 1)
            return UnityEngine.Object.Instantiate(objsGroup[0]);

        int i = UnityEngine.Random.Range(0, objsGroup.Length);
        return UnityEngine.Object.Instantiate(objsGroup[i]);
    }

    public static Transform GetRndChild(Transform hitEffectsHead)
    {
        if (hitEffectsHead.childCount== 0)
            return null;
        if (hitEffectsHead.childCount == 1)
            return hitEffectsHead.GetChild(0);

        int i = UnityEngine.Random.Range(0, hitEffectsHead.childCount);
        return hitEffectsHead.GetChild(i);
    }

    public static SpawnPath GetSpawnPosition(TeamTag team)
    {
        List<SpawnPath> playerSpawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPath>().ToList();
        List<SpawnPath> spawnPoints = playerSpawnPoints.FindAll(p => p.CheckTeam(team));
        SpawnPath s = null;

        int maxTries = 300;
        do
        {
            int i = UnityEngine.Random.Range(0, spawnPoints.Count);
            s = spawnPoints[i];
            maxTries = maxTries - 1;
            if (maxTries <= 0)
                break;
        } while (s == null || s.HasPlayer);
        return s;
    }
    public static CharaController GetCharaOfSlot(int slotID)
    {
        CharaSlot charaSlot = UnityEngine.Object.FindObjectsOfType<CharaSlot>().ToList().Find(s => s.GetID == slotID);
        if (charaSlot)
            return charaSlot.GetComponent<CharaController>();

        return null;
    }
    public static List<SpawnPath> GetPathsOfSlot(int slotID)
    {
        List<SpawnPath> allPaths = UnityEngine.Object.FindObjectsOfType<SpawnPath>().ToList();
        return allPaths.FindAll(s => s.CheckSlot(slotID));
    }
}