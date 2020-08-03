using UnityEngine;

public class TeamsSkinsSetter : MonoBehaviour
{
    PlayersRunDataSO data = null;
    void Start()
    {
        data = PlayersRunDataSO.Instance;
        if (data == null)
            return;

        if (!data.IsSP)
            return;


        Debug.Log("Adding Different Skins in SP is to be implemented yet");
    }
}