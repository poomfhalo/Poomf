using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public static class N_Extentions
{
    public static List<LoadablePrefab> prefabs
    {
        set
        {
            m_prefabs = value;
        }
        get
        {
            if(m_prefabs == null)
            {
                N_GameManager gm = Object.FindObjectOfType<N_GameManager>();
                if (gm == null)
                    return null;

                m_prefabs = gm.Prefabs;
            }
            return m_prefabs;
        }
    }
    private static List<LoadablePrefab> m_prefabs = new List<LoadablePrefab>();
    public static GameObject N_MakeObj(N_Prefab prefab, Vector3 pos, Quaternion rot, byte group = 0, object[] data = null)
    {
        LoadablePrefab p = GetPrefab(prefab);
        GameObject o = PhotonNetwork.Instantiate(p.name, pos, rot, group, data);
        return o;
    }
    public static GameObject MakeObj(N_Prefab prefab, Vector3 pos, Quaternion rot)
    {
        GameObject g = UnityEngine.Object.Instantiate(GetPrefab(prefab).LoadPrefab());
        g.transform.position = pos;
        g.transform.rotation = rot;
        return g;
    }
    public static GameObject N_MakeSceneObj(N_Prefab prefab, Vector3 pos, Quaternion rot, byte group = 0, object[] data = null)
    {
        LoadablePrefab p = GetPrefab(prefab);
        GameObject g = PhotonNetwork.InstantiateSceneObject(p.name, pos, rot, group, data);
        return g;
    }
    public static LoadablePrefab GetPrefab(N_Prefab prefab)
    {
        return prefabs.Single(f => f.type == prefab);
    }
}
