using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseRoom : MonoBehaviour
{
    public int GetRoomSlotID => roomSlot;

    [SerializeField] CustomizablePlayer player = null;
    [SerializeField] int roomSlot = 0;

    public void UpdateUsableSkin(CharaSkinData newData)=> player.SetNewSkinData(newData);
}