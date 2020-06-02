using UnityEngine;
using System;

[Serializable]
public class LoadablePrefab
{
    public N_Prefab type = N_Prefab.PlayerManager;
    public string name = "N_PlayerManager";
    public LoadablePrefab() { }
    public LoadablePrefab(N_Prefab type, string name)
    {
        this.type = type;
        this.name = name;
    }
    public GameObject LoadPrefab()
    {
        return Resources.Load<GameObject>(name);
    }
}
