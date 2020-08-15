using UnityEngine;

public class PlayerPlayStatsReader : PlayerPlayStatsDisplayer
{
    [Header("Reader Specifics")]
    [SerializeField] int chara = 0;

    void Start()
    {
        RefreshByData(PlayerPlayStatsGameHolder.instance.GetStats(chara));
    }
}