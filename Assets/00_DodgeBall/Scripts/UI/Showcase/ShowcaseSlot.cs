using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowcaseSlot : MonoBehaviour
{
    public int GetID => slotID;
    [SerializeField] int slotID = 0;
    [SerializeField] TextMeshProUGUI playerLevel = null;
    [SerializeField] TextMeshProUGUI playerName = null;

    public void SetPlayerData(int playerLevel, string playerName)
    {
        this.playerName.text = playerName;
        this.playerLevel.text = playerLevel.ToString();
    }
}