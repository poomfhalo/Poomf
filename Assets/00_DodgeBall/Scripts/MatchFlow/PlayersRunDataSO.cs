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
    public bool HasCharacter(int actorID)
    {
        foreach (var data in playersRunData)
        {
            if (data.actorID == actorID)
                return true;
        }
        return false;
    }
    public CharaSkinData GetSkinData(int actorNumber)
    {
        if (!HasCharacter(actorNumber))
        {
            Log.Warning("Attempted To Find Player Of ActorNum" + actorNumber + " But it is not added to the data object ");
            return null;
        }
        foreach (var data in playersRunData)
        {
            if (data.actorID == actorNumber)
                return data.charaSkinData.CreateSkinData();
        }
        Log.Warning("Should not be here, character was not found, and HasCharacter failed to detect it");
        return null;
    }

}