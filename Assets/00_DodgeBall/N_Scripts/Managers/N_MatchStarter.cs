using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class N_MatchStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] MatchTypeSelector matchSelector = null;
    public event Action onStartGame = null;
    bool reachedMaxPlayers
    {
        get
        {
            bool v = PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers;
            if(!PhotonNetwork.InRoom)
            {
                Log.LogL0("Not In Room");
            }
            if(PhotonNetwork.PlayerList.Length<PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                Log.LogL0("Still not enough players in the room, we want " + PhotonNetwork.CurrentRoom.MaxPlayers + " But we have " + PhotonNetwork.PlayerList.Length);
            }
            return v;
        }
    }

    public void CancelSearch()//Called, once we want to stop searching for the targeted match
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.AutomaticallySyncScene = false;
        Log.Message(PhotonNetwork.NickName + " Have Dissconnected, left room, and stopped syncing scenes");
    }
    public void PrepareGame()//is called from OnMasterEntered, to mak this script, prepare a match.
    {
        //Try to prepare the rooms/start the game, for now, just join, randomly, a single existing room.
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.LogL0("Failed To Join Random Room : Creating Room");
        matchSelector.GetMatchType(out MatchType matchType);
        RoomOptions ops = GetDefOptions(matchType);

        int fraction = DateTime.Now.Millisecond;
        string roomName = "M_" + PhotonNetwork.NickName + "_P_" + ops.MaxPlayers + "_ID_" + UnityEngine.Random.Range(int.MinValue, int.MaxValue) + fraction;
        PhotonNetwork.CreateRoom(roomName, ops);
    }
    public override void OnCreatedRoom()
    {
        Log.Message("Created room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        Log.Message("Joined room " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.AutomaticallySyncScene = true;

        if (reachedMaxPlayers && PhotonNetwork.IsMasterClient)
        {
            onStartGame?.Invoke();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log.Message(newPlayer.NickName + " have Entered room " + PhotonNetwork.CurrentRoom.Name);

        if (reachedMaxPlayers && PhotonNetwork.IsMasterClient)
        {
            onStartGame?.Invoke();
        }
    }

    public static RoomOptions GetDefOptions(MatchType matchType)
    {
        RoomOptions ops = new RoomOptions();
        byte maxPlayers = 2;

        switch (matchType)
        {
            case MatchType.OneVsOne:
                maxPlayers = 2;
                break;
            case MatchType.TwoVsTwo:
                maxPlayers = 4;
                break;
            case MatchType.ThreeVsThree:
                maxPlayers = 6;
                break;
            case MatchType.FourVsFour:
                maxPlayers = 8;
                break;
        }

        ops.MaxPlayers = maxPlayers;
        ops.IsVisible = true;
        ops.IsOpen = true;

        return ops;
    }
}