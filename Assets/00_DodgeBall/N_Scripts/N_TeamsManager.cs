using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class N_TeamsManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<int> actors = new List<int>();
    [SerializeField] List<TeamTag> tags = new List<TeamTag>();
    public void AddPlayer(TeamTag team, int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        tags.Add(team);
        actors.Add(actorNumber);
    }
}