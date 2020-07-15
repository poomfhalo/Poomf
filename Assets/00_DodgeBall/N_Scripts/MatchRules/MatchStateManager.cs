using UnityEngine;
using Photon.Pun;
using GW_Lib;
using Photon.Realtime;

public class MatchStateManager : N_Singleton<MatchStateManager>
{
    [SerializeField] MatchState matchState = null;

    [Header("Read Only")]
    public bool extCanPrepareOnStart = true;

    void Start()
    {
        if (extCanPrepareOnStart)
            PrerpareForGame();
    }

    public void PrerpareForGame()
    {
        ConnectToPlayers();
    }
    private void ConnectToPlayers()
    {
        TeamsManager.instance.AllCharacters.ForEach(c =>{
            c.GetComponent<CharaKnockoutPlayer>().E_OnKnockedOut += OnCharaKnockedOut;
        });
    }

    private void OnCharaKnockedOut(DodgeballCharacter charaKnockedOut)
    {
        GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty);

        bool isFinalRound = matchState.IsFinalRound();

        TeamTag winningTeam = TeamTag.A;
        if (isTeamAEmpty)
            winningTeam = TeamTag.B;
        matchState.AddWin(winningTeam);

        if (isFinalRound)
        {
            //Show Winning TeamTag Scene Thing/EndGame?
        }
        else
        {
            //MoveToNextRound And Update UI.
        }
    }

    private void GetEmptyTeams(out bool isTeamAEmpty,out bool isTeamBEmpty)
    {
        Team testTeam = TeamsManager.GetTeam(TeamTag.A);
        isTeamAEmpty = testTeam.IsEmpty;
        testTeam = TeamsManager.GetNextTeam(testTeam);
        isTeamBEmpty = testTeam.IsEmpty;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.InvokeDelayed(0.1f, () =>{
            GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty);

            MPTeam mpT = N_TeamsManager.GetMPTeam(otherPlayer.ActorNumber);
            mpT.CleanUp();

            if (isTeamAEmpty || isTeamBEmpty)
            {
                Log.Warning("Game Is over, one of the teams is empty");
                TeamTag winnerTeam = TeamTag.A;
                if (isTeamAEmpty)
                    winnerTeam = TeamTag.B;
                matchState.SetWinner(winnerTeam);
                PhotonNetwork.LoadLevel("Menu");
                PhotonNetwork.LeaveRoom();
            }
        });
    }
    public void StartNewGame() => matchState.StartNewGame();
}