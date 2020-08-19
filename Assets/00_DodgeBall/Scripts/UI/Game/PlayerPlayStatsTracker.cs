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