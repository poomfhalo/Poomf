using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

//this Must be placed on a game object that is always active, in the Menu Scene.
//this component, must also never be disabled. 
public class N_Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Button ready = null;
    [SerializeField] Button findingPlayers = null;
    [SerializeField] GameObject loginMenu = null;
    [SerializeField] TextMeshProUGUI playerNameText = null;
    [SerializeField] RegionSelector regionSelector = null;
    [SerializeField] MatchTypeSelector matchTypeSelector = null;

    N_MatchStarter matchStarter = null;
    bool wasReadyClicked = false;

    void Start()
    {
        ready.onClick.AddListener(OnReadyClicked);
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

        matchStarter = GetComponent<N_MatchStarter>();
        matchStarter.onStartGame += GoToRoom;
    }

    private void OnReadyClicked()
    {
        wasReadyClicked = true;
        ready.interactable = false;
        matchTypeSelector.GetMatchType(out MatchType type);
        if (type == MatchType.Practice)
        {
            SceneFader.instance.FadeIn(1, () => SceneManager.LoadScene("SP_Room"));
            return;
        }

        if(PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }
        if(regionSelector.GetRegion(out string region) && region != "DEF")
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

    public override void OnConnectedToMaster()
    {
        if (wasReadyClicked)
        {
            DisableEnteringGameUI();
            matchStarter.PrepareGame();
        }
        Log.Message("I Connected To Master " + PhotonNetwork.NickName + " Trying To Join RND Room in " + PhotonNetwork.CloudRegion);
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
}