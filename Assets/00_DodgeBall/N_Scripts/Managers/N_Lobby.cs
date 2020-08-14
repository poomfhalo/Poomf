using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using GW_Lib;
using System;

//this Must be placed on a game object that is always active, in the Menu Scene.
//this component, must also never be disabled. 
public class N_Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Button ready = null;
    [SerializeField] Button findingPlayers = null;
    [SerializeField] TextMeshProUGUI playerNameText = null;
    [SerializeField] RegionSelector regionSelector = null;
    [SerializeField] MatchTypeSelector matchTypeSelector = null;
    [SerializeField] PracticeMenu practiceMenu = null;

    N_MatchStarter matchStarter = null;
    bool wasReadyClicked = false;

    void Start()
    {
        practiceMenu.E_OnBack += OnPracticeBackPressed;
        PlayersRunDataSO.Instance.ClearOtherPlayersData();
        PlayersRunDataSO.Instance.localSkin = FindObjectOfType<CustomizablePlayer>().GetSkinData;

        ready.onClick.AddListener(OnReadyClicked);
        findingPlayers.onClick.AddListener(OnFindingPlayersClicked);
        findingPlayers.gameObject.SetActive(false);

        string n = PhotonNetwork.NickName;
        if (string.IsNullOrEmpty(n))
            n = "Random Name " + UnityEngine.Random.Range(-100, 100);
        playerNameText.text = n;
        PlayersRunDataSO.Instance.localPlayerName = playerNameText.text;
        PhotonNetwork.NickName = n;

        matchStarter = GetComponent<N_MatchStarter>();
        matchStarter.onStartGame += SpreadLocalData;
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnConnectedToMaster()
    {
        if (wasReadyClicked)
        {
            DisableEnteringGameUI();
            matchStarter.PrepareGame();
        }

        Log.Message("I Connected To Master " + PhotonNetwork.NickName + " Trying To Join RND Room in " + PhotonNetwork.CloudRegion);
    }

    private void OnReadyClicked()
    {
        wasReadyClicked = true;
        ready.interactable = false;
        matchTypeSelector.GetMatchType(out MatchType type);
        if (type == MatchType.Practice)
        {
            practiceMenu.Show();
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }
        if (regionSelector.GetRegion(out string region) && region != "DEF")
        {
            Log.LogL0("Trying to connect to region " + region);
            PhotonNetwork.ConnectToRegion(region);
        }
        else
        {
            Log.LogL0("Trying To Connect To Best Region");
            PhotonNetwork.ConnectUsingSettings();
        }

        regionSelector.SetInteractable(false);
        matchTypeSelector.SetInteractable(false);
    }

    private void OnPracticeBackPressed()
    {
        wasReadyClicked = false;
        ready.interactable = true;
    }
    private void OnFindingPlayersClicked()
    {
        findingPlayers.gameObject.SetActive(false);
        ready.gameObject.SetActive(true);
        wasReadyClicked = false;

        regionSelector.SetInteractable(true);
        matchTypeSelector.SetInteractable(true);

        matchStarter.CancelSearch();
    }
    private void DisableEnteringGameUI()
    {
        ready.interactable = true;
        ready.gameObject.SetActive(false);
        findingPlayers.gameObject.SetActive(true);
    }
    private void SpreadLocalData()
    {
        PlayersRunDataSO data = PlayersRunDataSO.Instance;
        data.ClearOtherPlayersData();//TODO: Consider Deleting this line?

        void SendLocalData()
        {
            data.localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

            CharaSkinDataPlain plain = new CharaSkinDataPlain(FindObjectOfType<CustomizablePlayer>().GetSkinData);

            Log.Message("Sending Customization Data Player Name :: " + data.localPlayerName);

            string s = "IDS : (";
            foreach (var id in plain.ids)
            {
                s = s + id + ",";
            }
            s = s + ")";
            Log.Message(s);

            photonView.RPC("RecieveLocalData", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber, data.localPlayerName,
                plain.types, plain.ids, plain.reds, plain.greens, plain.blues, plain.colorIndicies, plain.texesIndicies);
        }
        this.InvokeDelayed(3, SendLocalData);
    }
    [PunRPC]
    private void RecieveLocalData(int actorID, string playerName, int[] types, int[] ids, float[] reds, float[] greens, float[] blues, int[] colorIndicies, int[] texesIndicies)
    {
        PlayersRunDataSO data = PlayersRunDataSO.Instance;
        CharaSkinDataPlain plainSkin = new CharaSkinDataPlain(types, ids, reds, greens, blues, colorIndicies, texesIndicies);

        Log.Message("Recieved Customization Data Player Name :: " + playerName);
        string s = "IDS : (";
        foreach (var id in ids)
        {
            s = s + id + ",";
        }
        s = s + ")";
        Log.Message(s);

        data.AddPlayerRunData(actorID, plainSkin, playerName);

        if (PhotonNetwork.PlayerList.Length == data.playersRunData.Count)
            GoToRoom();
    }
    private void GoToRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Log.Message("Loading MP_Room");
            SceneFader.instance.FadeIn(1.2f, () => PhotonNetwork.LoadLevel("MP_Room"));
        }
        else
        {
            SceneFader.instance.FadeIn(1, null);
        }
    }
}