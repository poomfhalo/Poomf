using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedPlayerSlotsUI : MonoBehaviour
{
    [SerializeField] List<ShowcaseSlot> slots = new List<ShowcaseSlot>();

    [Header("Read Only")]
    [SerializeField] List<ShowcaseRoom> rooms = new List<ShowcaseRoom>();

    PlayersRunDataSO playerRunData = null;

    void Awake()
    {
        FindObjectsOfType<CustomizablePlayer>().ToList().ForEach(c => c.extLoadOnStart = false);
    }
    void Start()
    {
        playerRunData = PlayersRunDataSO.Instance;

        if (slots.Count == 0)
            slots = GetComponentsInChildren<ShowcaseSlot>().ToList();

        rooms = FindObjectsOfType<ShowcaseRoom>().ToList();

        UseFilledData();
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
                Log.Message("No room, Or no Slot");
                continue;
            }

            room.UpdateUsableSkin(p.charaSkinData.CreateSkinData());
            slot.SetPlayerData(11, p.charaName);
        }
    }
}