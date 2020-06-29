using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using GW_Lib;

//this Must be placed on a game object that is always active.
//this component, must also never be disabled. 
public class N_Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Button ready = null;
    [SerializeField] Button tutorial = null;
    [SerializeField] ToggleButtonGroup regionsGroup = null;
    [SerializeField] Button findingPlayers = null;
    [SerializeField] GameObject loginMenu = null;

    void Start()
    {
        ready.onClick.AddListener(OnReadyClicked);
        tutorial.onClick.AddListener(OnTutorialClicked);
        findingPlayers.onClick.AddListener(OnFindingPlayersClicked);
        findingPlayers.gameObject.SetActive(false);
        if (PhotonNetwork.IsConnectedAndReady)
            loginMenu.SetActive(false);
    }

    private void OnReadyClicked()
    {
        ready.interactable = false;

        if (regionsGroup.ActiveButton.text == "DEF")
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.ConnectToRegion(regionsGroup.ActiveButton.text);
        }
    }
    private void OnTutorialClicked()
    {
        SceneManager.LoadScene("SP Game");
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Log.Message("I Connected To Master " + PhotonNetwork.NickName + " Trying To Join RND Room");
        PhotonNetwork.JoinRandomRoom();
        loginMenu.SetActive(false);

        ready.interactable = true;
        ready.gameObject.SetActive(false);
        findingPlayers.gameObject.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.Message("Failed To Join Random Room : Creating Room");
        RoomOptions ops = new RoomOptions();
        ops.MaxPlayers = 2;
        ops.IsOpen = true;
        ops.IsVisible = true;
        int fraction = DateTime.Now.Millisecond;
        string roomName = "M_" + PhotonNetwork.NickName + "_P_" + ops.MaxPlayers + "_ID_" + UnityEngine.Random.Range(int.MinValue, int.MaxValue) + fraction;
        PhotonNetwork.CreateRoom(roomName, ops);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Log.Message("Created room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log.Message(newPlayer.NickName + " have Entered room " + PhotonNetwork.CurrentRoom.Name);
        if(PhotonNetwork.PlayerList.Length>=PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            Log.Message("Loading MP Game");
            PhotonNetwork.LoadLevel("MP Game");
        }
    }
    private void OnFindingPlayersClicked()
    {
        findingPlayers.interactable = false;
        this.InvokeDelayed(0.1f,()=> {
            findingPlayers.gameObject.SetActive(false);
            findingPlayers.interactable = true;
            ready.gameObject.SetActive(true);
        });

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.AutomaticallySyncScene = false;
    }
}