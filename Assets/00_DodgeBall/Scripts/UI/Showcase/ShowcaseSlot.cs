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
        if(this.playerName)
            this.playerName.text = playerName;

        if(this.playerLevel)
            this.playerLevel.text = playerLevel.ToString();
    }
}