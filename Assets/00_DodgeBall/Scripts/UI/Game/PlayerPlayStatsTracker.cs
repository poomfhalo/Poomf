using TMPro;
using UnityEngine;

public class PlayerPlayStatsTracker : PlayerPlayStatsDisplayer
{
    [Header("Tracker Uniques")]
    [SerializeField] TextMeshProUGUI nameText = null;
    int localPlayerID = 0;
    PlayerRoundStatsCollector collector = null;

    void Start()
    {
        GameIntroManager.instance.OnEntryCompleted += OnGameStatred;
        if(PlayersRunDataSO.Instance)
            nameText.text = PlayersRunDataSO.Instance.localPlayerName;
    }
    void OnDestroy()
    {
        if(GameIntroManager.instance)
            GameIntroManager.instance.OnEntryCompleted -= OnGameStatred;
    }

    private void OnGameStatred()
    {
        localPlayerID = PlayersRunDataSO.Instance.localPlayerID;
        print("Local Player ID " + localPlayerID);
        foreach (var chara in TeamsManager.instance.AllCharacters)
        {
            print("Found Character Of ID " + chara.GetID());
            if(localPlayerID == chara.GetID())
            {
                collector = chara.GetComponent<PlayerRoundStatsCollector>();
                break;
            }
        }
        collector.OnDataUpdated += ()=> RefreshByData(collector.GetRoundStats);
    }
}