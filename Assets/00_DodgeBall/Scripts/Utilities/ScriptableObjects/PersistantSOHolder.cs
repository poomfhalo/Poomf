using System.Collections.Generic;
using GW_Lib.Utility;
using UnityEngine;

public class PersistantSOHolder : Singleton<PersistantSOHolder>
{
    [SerializeField] List<ScriptableObject> objs = new List<ScriptableObject>();

    public void SetObjs(List<PersistantSO> recievedObjs)
    {
        objs.RemoveAll(o => o == null);
        foreach (var r in recievedObjs)
        {
            if (objs.Contains(r))
                continue;
            objs.Add(r);
        }
    }
}