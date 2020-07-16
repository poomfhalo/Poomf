using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using GW_Lib;
using TMPro;

//this Must be placed on a game object that is always active.
//this component, must also never be disabled. 
public class N_Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Button ready = null;
    [SerializeField] Button tutorial = null;
    [SerializeField] ToggleButtonGroup regionsGroup = null;
    [SerializeField] Button findingPlayers = null;
    [SerializeField] GameObject loginMenu = null;
    [SerializeField] TextMeshProUGUI playerNameText = null;

    bool reachedMaxPlayers => PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers;

    void Start()
    {
        ready.onClick.AddListener(OnReadyClicked);
        tutorial.onClick.AddListener(OnTutorialClicked);
        findingPlayers.onClick.AddListener(OnFindingPlayersClicked);
        findingPlayers.gameObject.SetActive(false);
        if (PhotonNetwork.IsConnectedAndReady || !string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            loginMenu.SetActive(false);
            playerNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            loginMenu.GetComponent<N_LoginMenu>().onNameSet += (n) => playerNameText.text = n;
        }
    }

    private void OnReadyClicked()
    {
        ready.interactable = false;
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PrepareRoom();
            return;
        }
        if (regionsGroup.ActiveButton.text == "DEF")
        {
            Log.LogL0("Trying To Connect To Best Region");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            string region = regionsGroup.ActiveButton.text.ToLower();
            Log.LogL0("Trying to connect to region " + region);
            PhotonNetwork.ConnectToRegion(region);
        }
        regionsGroup.SetInteractable(false);
    }
    private void OnTutorialClicked()
    {
        SceneFader.instance.FadeIn(1, () => SceneManager.LoadScene("SP_Room"));
    }
    public override void OnConnectedToMaster()
    {
        if (!ready.interactable)
        {
            Log.Message("I Connected To Master " + PhotonNetwork.NickName + " Trying To Join RND Room in " + PhotonNetwork.CloudRegion);
            PrepareRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.LogL0("Failed To Join Random Room : Creating Room");
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
        Log.Message("Created room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        Log.Message("Joined room " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.AutomaticallySyncScene = true;
        if(reachedMaxPlayers)
        {
            GoToRoom();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log.Message(newPlayer.NickName + " have Entered room " + PhotonNetwork.CurrentRoom.Name);

        if (reachedMaxPlayers && PhotonNetwork.IsMasterClient)
        {
            GoToRoom();
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
        Log.Message(PhotonNetwork.NickName + " Have Dissconnected, left room, and stopped syncing scenes");
        regionsGroup.SetInteractable(true);
    }
    private void PrepareRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        loginMenu.SetActive(false);

        ready.interactable = true;
        ready.gameObject.SetActive(false);
        findingPlayers.gameObject.SetActive(true);
    }
    private void GoToRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Log.Message("Loading MP_Room");
            SceneFader.instance.FadeIn(1.3f, () => PhotonNetwork.LoadLevel("MP_Room"));
        }
        else
        {
            SceneFader.instance.FadeIn(1, null);
        }
    }

    public static RoomOptions GetDefOptions()
    {
        RoomOptions ops = new RoomOptions();
        ops.MaxPlayers = 6;
        ops.IsVisible = true;
        ops.IsOpen = true;
        return ops;
    }
}