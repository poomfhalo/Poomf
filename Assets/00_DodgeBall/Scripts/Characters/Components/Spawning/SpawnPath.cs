using UnityEngine;
using Photon.Pun;

public enum PathType { GameStartPath }
public class SpawnPath : MonoBehaviourPunCallbacks
{
    public PathType pathType = PathType.GameStartPath;

    public Transform[] GetPoints => points;
    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;
    public bool HasPlayer => hasPlayer;
    public int SlotId => GetCharaSlotData().id;

    [Tooltip("What Slot, does this spawn point links to")]
    [SerializeField] int slotID = 0;
    [SerializeField] Transform[] points = new Transform[0];
    [Header("Read Only")]
    [SerializeField] bool hasPlayer = false;

    [PunRPC]
    public void Fill()
    {
        hasPlayer = true;
    }
    [PunRPC]
    public void Free()
    {
        hasPlayer = false;
    }

    public bool CheckSlot(int slotID) => SlotId == slotID;
    public bool CheckTeam(TeamTag t) => t == GetCharaSlotData().team;
    
    CharaSlotData GetCharaSlotData() => DodgeballGameManager.instance.gameSlotsData.GetData(slotID);
}