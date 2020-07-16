using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using GW_Lib;

public class N_MatchStateManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        MatchStateManager.instance.ResultLoadFunc = RoundEndLoad;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.InvokeDelayed(0.1f, () => {
            TeamsManager.GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty);

            MPTeam mpT = N_TeamsManager.GetMPTeam(otherPlayer.ActorNumber);
            mpT.CleanUp();

            if (isTeamAEmpty || isTeamBEmpty)
            {
                Log.Warning("Game Is over, one of the teams is empty");
                TeamTag winnerTeam = TeamTag.A;
                if (isTeamAEmpty)
                    winnerTeam = TeamTag.B;

                MatchState.Instance.SetWinner(winnerTeam);
                PhotonNetwork.LoadLevel("Menu");
                PhotonNetwork.LeaveRoom();
            }
        });
    }
    private IEnumerator RoundEndLoad(bool isGameOver)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            SceneFader.instance.FadeIn(1.1f, null);
            yield return new WaitForSeconds(1.1f);
            if(isGameOver)
            {
                PhotonNetwork.LoadLevel("MP_MatchResult");
            }
            else
            {
                PhotonNetwork.LoadLevel("MP_RoundResult");
            }
        }
        else
        {
            SceneFader.instance.FadeIn(1f, null);
            yield return new WaitForSeconds(1);
        }
    }
}
