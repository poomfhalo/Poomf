using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class N_Room : MonoBehaviourPunCallbacks
{
    [Header("Constants")]
    [SerializeField] Image levelBGImage = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [Header("Variables")]
    [Tooltip("In Seconds")]
    [SerializeField] float levelChangeTimer = 30;

    VoteSlot[] delegators = null;
    float levelChangeCounter = 0;
    bool isTimerRunning = false;
    Coroutine loadLevelCoro = null;

    void Awake()
    {
        delegators = GetComponentsInChildren<VoteSlot>();
        foreach (var d in delegators)
        {
            d.E_OnPointerEntered += OnEntered;
        }
        levelChangeCounter = levelChangeTimer;
        isTimerRunning = true;
    }
    void Update()
    {
        if (!isTimerRunning)
            return;
        levelChangeCounter = levelChangeCounter - Time.deltaTime;
        int minutes = Mathf.FloorToInt(levelChangeCounter) / 60;
        int seconds = Mathf.FloorToInt(levelChangeCounter) % 60;
        seconds = Mathf.FloorToInt(Mathf.Clamp(seconds, 0, levelChangeTimer));

        string m = "";
        string s = "";

        if(minutes<10)
        {
            m = "0" + minutes;
        }
        else
        {
            m = minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + seconds;
        }
        else
        {
            s = seconds.ToString();
        }
        timerText.text = m + ":" + s;

        if (levelChangeCounter<=0)
        {
            isTimerRunning = false;
            if(loadLevelCoro == null)
                loadLevelCoro = StartCoroutine(TryLoadLevel());
        }
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

    private void OnEntered(VoteSlot s)
    {
        levelBGImage.sprite = s.CoverSprite;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log.Warning("Player Has Disconnected");
    }
}