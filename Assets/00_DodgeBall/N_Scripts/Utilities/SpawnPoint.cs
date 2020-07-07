using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPunCallbacks
{
    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;
    public bool HasPlayer => hasPlayer;
    public TeamTag BelongsTo => belongsTo;
    public Transform LinkedTo => linkedTo;

    [SerializeField] TeamTag belongsTo = TeamTag.A;
    [SerializeField] Transform linkedTo = null;
    [Header("Read Only")]
    [SerializeField] bool hasPlayer = false;
    [SerializeField] int activePlayer = -1;

    [PunRPC]
    public void Fill(int actorNumber)
    {
        hasPlayer = true;
        activePlayer = actorNumber;
    }
    [PunRPC]
    public void Free()
    {
        hasPlayer = false;
        activePlayer = -1;
    }

    public bool CheckPlayer(int actorNumber)
    {
        return HasPlayer && actorNumber == activePlayer && actorNumber != -1;
    }
    public bool CheckTeam(TeamTag t)
    {
        return t == BelongsTo;
    }
}