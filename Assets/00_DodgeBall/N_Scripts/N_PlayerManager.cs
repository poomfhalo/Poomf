using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class N_PlayerManager : MonoBehaviourPunCallbacks
{
    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        if (photonView.IsMine)
        {
            yield return StartCoroutine(SpawnPC());
        }
    }

    IEnumerator SpawnPC()
    {
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        SpawnPoint s = playerSpawnPoints.Find(p => p.CheckPlayer(photonView.Controller.ActorNumber));
        if (s == null)
        {
            int maxTries = 60;
            do
            {
                int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
                s = playerSpawnPoints[i];
                maxTries = maxTries - 1;
                if (maxTries <= 0)
                    break;
            } while (s == null || s.CheckPlayer(photonView.Controller.ActorNumber));
            s.GetComponent<PhotonView>().RPC("Fill", RpcTarget.All, photonView.Controller.ActorNumber);
        }
        yield return 0;

        GameObject g = N_Extentions.N_MakeObj(N_Prefab.Player, s.position, s.rotation);
        yield return new WaitForSeconds(0.1f);
        g.transform.SetParent(transform);
        Debug.Log(photonView.Controller + " Created a PC ", g);
    }
}
