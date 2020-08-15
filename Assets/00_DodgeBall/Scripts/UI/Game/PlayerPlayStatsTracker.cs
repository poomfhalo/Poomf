using System.Linq;

public class PlayerPlayStatsTracker : PlayerPlayStatsDisplayer
{
    int localPlayerID = 0;
    PlayerRoundStatsCollector collector = null;

    void Start()
    {
        GameIntroManager.instance.OnEntryCompleted += OnGameStatred;
    }
    void OnDestroy()
    {
        if(GameIntroManager.instance)
            GameIntroManager.instance.OnEntryCompleted -= OnGameStatred;
    }

    private void OnGameStatred()
    {
        localPlayerID = PlayersRunDataSO.Instance.localPlayerID;
        collector = FindObjectsOfType<CharaSlot>().ToList().Find(c => c.GetID == localPlayerID).GetComponent<PlayerRoundStatsCollector>();
        collector.OnDataUpdated += ()=> RefreshByData(collector.GetRoundStats);
    }
}