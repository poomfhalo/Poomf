using UnityEngine;

public class TeamsSkinsSetter : MonoBehaviour
{
    PlayersRunDataSO dataSO = null;
    void Start()
    {
        dataSO = PlayersRunDataSO.Instance;
        if (dataSO == null)
            return;

        if (!dataSO.IsSP)
            return;


        //PlayerRunData data = dataSO.GetPlayerRunData(photonView.ControllerActorNr);
        //CharaSkinData skinData = data.charaSkinData.CreateSkinData();
        //pc.GetComponentInChildren<CustomizablePlayer>().SetNewSkinData(skinData);

        Debug.Log("Adding Different Skins in SP is to be implemented yet");
    }
}