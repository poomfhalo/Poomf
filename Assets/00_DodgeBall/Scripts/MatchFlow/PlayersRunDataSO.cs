using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dodgeball/PlayersRunDataSO",fileName = "PlayersRunDataSO")]
public class PlayersRunDataSO : PersistantSO
{
    [Serializable]
    public class PlayerRunData
    {
        public int actorID = -1;
        public string charaName = "";
        public CharaSkinDataPlain charaSkinData = null;

        public PlayerRunData() { }
        public PlayerRunData(int actorID, CharaSkinDataPlain charaSkinData,string charaName)
        {
            this.charaSkinData = charaSkinData;
            this.actorID = actorID;
            this.charaName = charaName;
        }
    }

    public static PlayersRunDataSO Instance => Resources.Load<PlayersRunDataSO>("PlayersRunDataSO");

    [Header("Read Only")]
    public List<PlayerRunData> playersRunData = new List<PlayerRunData>();
    public bool IsSP => playersRunData.Count == 0;
    public string localPlayerName = "";
    public CharaSkinData localSkin = null;

    public void ClearOtherPlayersData()
    {
        playersRunData.Clear();
    }

    public void AddPlayerRunData(int actorID, CharaSkinDataPlain charaSkinData,string charaName)
    {
        PlayerRunData rData = new PlayerRunData(actorID, charaSkinData, charaName);
        playersRunData.Add(rData);
    }
    public bool HasCharacter(int slotID)
    {
        foreach (var data in playersRunData)
        {
            if (data.actorID == slotID)
                return true;
        }
        return false;
    }
}