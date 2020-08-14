using UnityEngine;

public class SP_Room : MonoBehaviour
{
    [SerializeField] SP_MatchData matchData = null;
    PlayersRunDataSO data = null;

    void Awake()
    {
        data = PlayersRunDataSO.Instance;
        data.ClearOtherPlayersData();

        PopulateRunData();
    }

    private void PopulateRunData()
    {
        PlayersRunDataSO so = PlayersRunDataSO.Instance;
        int actorID = 0;

        foreach (var p in matchData.teamA)
        {
            if (matchData.playerActorID == actorID)
            {
                so.AddPlayerRunData(actorID, so.localSkin, so.localPlayerName);
                actorID = actorID + 2;
                continue;
            }
            so.AddPlayerRunData(actorID, p.skinData, p.charaName);
            actorID = actorID + 2;
        }
        actorID = 1;

        foreach (var p in matchData.teamB)
        {
            if (matchData.playerActorID == actorID)
            {
                so.AddPlayerRunData(actorID, so.localSkin, so.localPlayerName);
                actorID = actorID + 2;
                continue;
            }
            so.AddPlayerRunData(actorID, p.skinData, p.charaName);
            actorID = actorID + 2;
        }

        so.localPlayerID = matchData.playerActorID;
    }
}