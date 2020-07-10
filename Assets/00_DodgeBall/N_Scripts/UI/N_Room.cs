using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class N_Room : MonoBehaviourPunCallbacks
{
    LevelsRoom room = null;

    void Start()
    {
        room = GetComponent<LevelsRoom>();
        room.extLoadOnClick = false;
        room.TryLoadLevelFunc = TryLoadLevel;
    }
    private IEnumerator TryLoadLevel()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnected);
        if (PhotonNetwork.IsMasterClient)
        {
            SceneFader.instance.FadeIn(1,()=> {
                string winner = GetComponent<N_VotesBox>().GetMostVotedScene();
                PhotonNetwork.LoadLevel(winner);
            });
        }
        else
        {
            SceneFader.instance.FadeIn(0.9f,null);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log.Warning("Player Has Disconnected");
    }
}