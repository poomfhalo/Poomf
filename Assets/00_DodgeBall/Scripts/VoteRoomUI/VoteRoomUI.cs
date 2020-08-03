using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoteRoomUI : MonoBehaviour
{
    [SerializeField] List<ShowcaseSlot> slots = new List<ShowcaseSlot>();

    [Header("Read Only")]
    [SerializeField] List<ShowcaseRoom> rooms = new List<ShowcaseRoom>();

    PlayersRunDataSO playerRunData = null;
    void Start()
    {
        playerRunData = PlayersRunDataSO.Instance;

        if (slots.Count == 0)
            slots = GetComponentsInChildren<ShowcaseSlot>().ToList();

        rooms = FindObjectsOfType<ShowcaseRoom>().ToList();
        if (playerRunData.IsSP)
            UseClearData();
        else
            UseFilledData();
    }

    private void UseClearData()
    {
        ShowcaseSlot slot = slots.Find(s => s.GetID == 0);
        slot.SetPlayerData(99, playerRunData.localPlayerName);
        ShowcaseRoom room = rooms.Find(r => r.GetRoomSlotID == 0);
        room.UpdateUsableSkin(playerRunData.localSkin);
    }

    private void UseFilledData()
    {
        slots.ForEach(s =>{
            if (!playerRunData.HasCharacter(s.GetID))
                s.gameObject.SetActive(false);
        });
        slots.RemoveAll(s => !s.gameObject.activeSelf);

        foreach (var p in playerRunData.playersRunData)
        {
            ShowcaseSlot slot = slots.Find(s => s.GetID == p.actorID);
            ShowcaseRoom room = rooms.Find(r => r.GetRoomSlotID == p.actorID);

            if (room == null || slot == null)
            {
                Debug.Log("No room, Or no Slot");
                continue;
            }

            room.UpdateUsableSkin(p.charaSkinData.CreateSkinData());
            slot.SetPlayerData(11, p.charaName);
            Debug.Log("Updating Data");
        }
        Debug.Log("WT?");
    }
}