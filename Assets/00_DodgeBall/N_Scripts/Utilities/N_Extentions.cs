using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

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
                N_GameManager gm = UnityEngine.Object.FindObjectOfType<N_GameManager>();
                if (gm == null)
                    return null;

                m_prefabs = gm.Prefabs;
            }
            return m_prefabs;
        }
    }

    private static List<LoadablePrefab> m_prefabs = new List<LoadablePrefab>();

    public static RaiseEventOptions GetDefOps
    {
        get
        {
            RaiseEventOptions ops = new RaiseEventOptions();
            ops.Receivers = ReceiverGroup.All;
            return ops;
        }
    }

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

    public static DodgeballCharacter GetCharacter(int actorNumber)
    {
        DodgeballCharacter chara = null;
        chara = TeamsManager.instance.AllCharacters.Find(c =>{
            PhotonView pv = c.GetComponent<PhotonView>();
            if (!c.GetComponent<PhotonView>())
                return false;
            return pv.Controller.ActorNumber == actorNumber;
        });
        return chara;
    }
    public static T FindMyNetworkedObj<T>() where T : UnityEngine.Component
    {
        T[] tes = UnityEngine.Object.FindObjectsOfType<T>();
        foreach (var t in tes)
        {
            if (!t.GetComponent<PhotonView>())
                continue;
            if (t.GetComponent<PhotonView>().IsMine)
                return t;
        }
        return null;
    }
    public static T FindNetworkedObj<T>(int actorNumber) where T : UnityEngine.Component
    {
        T[] tes = UnityEngine.Object.FindObjectsOfType<T>();
        foreach (var t in tes)
        {
            if (!t.GetComponent<PhotonView>())
                continue;
            if (t.GetComponent<PhotonView>().Controller.ActorNumber == actorNumber)
                return t;
        }
        return null;
    }
    public static void N_RaiseEvent(byte b, object content, bool raiseOnMasterOnly = true)
    {
        if (raiseOnMasterOnly && !PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.RaiseEvent(b, content, GetDefOps, SendOptions.SendReliable);
    }
}