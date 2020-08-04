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
    public static T Make<T>(T[] objsGroup, int index = -1) where T : UnityEngine.Object
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
        if (hitEffectsHead.childCount == 0)
            return null;
        if (hitEffectsHead.childCount == 1)
            return hitEffectsHead.GetChild(0);

        int i = UnityEngine.Random.Range(0, hitEffectsHead.childCount);
        return hitEffectsHead.GetChild(i);
    }

    public static void PlayChildEffect(Transform head)
    {
        Transform child = GetRndChild(head);
        child.GetComponent<ParticleSystem>().Play(true);
    }
    public static CharaController GetCharaOfSlot(int slotID)
    {
        CharaSlot charaSlot = UnityEngine.Object.FindObjectsOfType<CharaSlot>().ToList().Find(s => s.GetID == slotID);
        if (charaSlot)
            return charaSlot.GetComponent<CharaController>();

        return null;
    }

    public static void StripFloat(float f, out int ints, out float frac)
    {
        ints = (int)f;
        frac = f - ints;
    }

    /// <summary>
    /// Sets the charas lock.
    /// </summary>
    /// <param name="lockState">If set to <c>true</c> lock state.</param>
    /// <param name="slots">Setting as null, means all characters</param>
    public static void SetCharasLock(bool lockState, int[] slots)
    {
        if (slots == null)
        {
            CharaController[] charas = UnityEngine.Object.FindObjectsOfType<CharaController>();
            foreach (var chara in charas)
            {
                chara.SetLockTo(lockState);
            }

            return;
        }

        foreach (var slot in slots)
        {
            CharaController chara = GetCharaOfSlot(slot);
            if (chara == null)
                continue;

            chara.SetLockTo(lockState);
        }
    }

    public static CharaPath GetSpawnPosition(TeamTag team)
    {
        List<CharaPath> playerSpawnPoints = UnityEngine.Object.FindObjectsOfType<CharaPath>().ToList();
        List<CharaPath> spawnPoints = playerSpawnPoints.FindAll(p => p.CheckTeam(team));
        spawnPoints.RemoveAll(p => !p.CheckTag(PathType.GameStartPath));
        CharaPath s = null;

        int maxTries = 300;
        while (s == null || s.HasPlayer)
        {
            int i = UnityEngine.Random.Range(0, spawnPoints.Count);
            s = spawnPoints[i];
            Log.LogL0("Sending Position " + s.name);
            if (s.HasPlayer)
            {
                Log.LogL0("Failed To Send Position " + s.name + " It Has Player ");
                s = null;
                continue;
            }
            maxTries = maxTries - 1;
            if (maxTries <= 0)
            {
                Log.Error("failed to find Random Position?");
                break;
            }
        }
        return s;
    }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <returns>The path.</returns>
    /// <param name="slotID">Slot identifier.</param>
    /// <param name="tag">Tag.</param>
    /// <param name="index">Index, use -1 for random path, 0 for first path,note value is clampped correctly</param>
    public static CharaPath GetPath(int slotID, PathType tag, int index)
    {
        List<CharaPath> allPaths = UnityEngine.Object.FindObjectsOfType<CharaPath>().ToList();
        List<CharaPath> goodPaths = allPaths.FindAll(p => p.CheckSlot(slotID) && p.CheckTag(tag));
        if (goodPaths.Count == 0)
        {
            return null;
        }
        if (goodPaths.Count == 1)
        {
            return goodPaths[0];
        }
        if (index == -1)
        {
            index = UnityEngine.Random.Range(0, goodPaths.Count);
        }
        index = Mathf.Clamp(index, 0, goodPaths.Count - 1);
        return goodPaths[index];
    }
    public static CharaPath GetPath(int slotID, PathType tag, string pathName)
    {
        List<CharaPath> allPaths = UnityEngine.Object.FindObjectsOfType<CharaPath>().ToList();
        List<CharaPath> goodPaths = allPaths.FindAll(p => p.CheckSlot(slotID) && p.CheckTag(tag));
        if (goodPaths.Count == 0)
            return null;
        if (goodPaths.Count == 1)
            return goodPaths[0];

        foreach (var p in goodPaths)
        {
            if (p.name == pathName)
                return p;
        }

        return null;
    }
}