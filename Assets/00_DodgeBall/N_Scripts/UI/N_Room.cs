using System.Collections;
using GW_Lib;
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
                photonView.RPC("DoLoadLevel", RpcTarget.AllViaServer, winner);
            });
        }
        else
        {
            SceneFader.instance.FadeIn(0.9f,null);
        }
    }
    [PunRPC]
    private void DoLoadLevel(string targetLevel)
    {
        MatchState.Instance.StartNewGame(targetLevel);
        if (PhotonNetwork.IsMasterClient)
        {
            this.InvokeDelayed(0.3f, () => {
                PhotonNetwork.LoadLevel(targetLevel);
            });
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log.Warning("Player Has Disconnected");
    }
}