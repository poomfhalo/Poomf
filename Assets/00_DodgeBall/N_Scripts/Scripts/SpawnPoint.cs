using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPunCallbacks
{
    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;
    public bool HasSpawned => hasSpawned;
    
    [Header("Read Only")]
    [SerializeField] bool hasSpawned = false;
    [SerializeField] int activePlayer = -1;

    [PunRPC]
    public void Fill(int playerID)
    {
        hasSpawned = true;
        activePlayer = playerID;
    }
    public bool CheckPlayer(int actorNumber)
    {
        return HasSpawned && actorNumber == activePlayer && actorNumber != -1;
    }
    [PunRPC]
    public void Free()
    {
        hasSpawned = false;
        activePlayer = -1;
    }
}