using Photon.Pun;
using UnityEngine;

public class CharaSlot : MonoBehaviour
{
    public int GetID => activeSlot.id;
    public TeamTag GetTeam => activeSlot.team;
    public bool setActiveOnStart = true;
    [Tooltip("To be Set Manually in SinglePlayer, and set up by code in multiplayer")]
    [SerializeField] int id = 0;

    [Header("Read Only")]
    [SerializeField] bool wasSetUp = false;
    [SerializeField] CharaSlotData activeSlot = new CharaSlotData();

    void Start()
    {
        if (setActiveOnStart)
        {
            SetUp(id);
            GetComponent<DodgeballCharacter>().PrepareForGame();
        }
    }

    [PunRPC]
    public void SetUp(int id)
    {
        activeSlot = DodgeballGameManager.instance.gameSlotsData.GetData(id);
        this.id = id;
        wasSetUp = true;

        CharaPath path = GameExtentions.GetPath(id, PathType.GameStartPath, 0);
        path.Fill();
    }
}