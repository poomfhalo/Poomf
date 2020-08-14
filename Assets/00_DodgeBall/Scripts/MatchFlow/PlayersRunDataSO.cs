using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dodgeball/PlayersRunDataSO",fileName = "PlayersRunDataSO")]
public class PlayersRunDataSO : PersistantSO
{
    public static PlayersRunDataSO Instance => Resources.Load<PlayersRunDataSO>("PlayersRunDataSO");

    [Header("Read Only")]
    public List<PlayerRunData> playersRunData = new List<PlayerRunData>();
    public bool IsSP => playersRunData.Count == 0;
    public string localPlayerName = "";
    public CharaSkinData localSkin = null;
    public int localPlayerID = 0;

    public void ClearOtherPlayersData()
    {
        playersRunData.Clear();
    }

    public void AddPlayerRunData(int actorID, CharaSkinDataPlain charaSkinData,string charaName)
    {
        PlayerRunData rData = new PlayerRunData(actorID, charaSkinData, charaName);
        playersRunData.Add(rData);
    }
    public void AddPlayerRunData(int actorID, CharaSkinData charaSkinData, string charaName)
    {
        AddPlayerRunData(actorID, new CharaSkinDataPlain(charaSkinData), charaName);
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
        PlayerRunData data = GetPlayerRunData(actorNumber);
        if (data == null)
            return null;
        return data.charaSkinData.CreateSkinData();
    }
    public PlayerRunData GetPlayerRunData(int actorNumber)
    {
        if (!HasCharacter(actorNumber))
        {
            Log.Warning("Attempted To Find Player Of ActorNum" + actorNumber + " But it is not added to the data object ");
            return null;
        }
        foreach (var data in playersRunData)
        {
            if (data.actorID == actorNumber)
                return data;
        }
        Log.Warning("Should not be here, character was not found, and HasCharacter failed to detect it");
        return null;
    }
}