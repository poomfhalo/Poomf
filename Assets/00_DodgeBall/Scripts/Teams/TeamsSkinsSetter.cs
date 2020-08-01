using UnityEngine;

public class TeamsSkinsSetter : MonoBehaviour
{
    PlayersRunDataSO data = null;
    void Start()
    {
        data = PlayersRunDataSO.Instance;
        if (data == null)
            return;

        if (data.IsSP)
            return;


        foreach (var p in data.playersRunData)
        {
            N_PC playerObj = N_TeamsManager.GetPlayer(p.actorID);

            if (playerObj == null)
                continue;
            playerObj.GetComponentInChildren<CustomizablePlayer>().SetNewSkinData(p.charaSkinData);
        }
    }
}