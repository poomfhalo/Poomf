using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerRoundStats
{
    public int passes = 0;
    public int throws = 0;
    public int revivies = 0;

    public int knocks = 0;
    public int deaths = 0;
    public string GetKD() => knocks + " : " + deaths;
}

public class PlayerRoundStatsCollector : MonoBehaviour
{
    [Header("Read Only")]
    [SerializeField] PlayerRoundStats roundStats = null;

    DodgeballCharacter chara = null;
    BallLauncher launcher = null;
    CharaHitPoints hp = null;

    void Start()
    {
        chara = GetComponent<DodgeballCharacter>();
        launcher = GetComponent<BallLauncher>();
        hp = GetComponent<CharaHitPoints>();

        launcher.E_OnThrowPointReached += OnThrowPointReached;
        hp.OnZeroHP += OnZeroHp;
    }
    void OnDestroy()
    {
        if(launcher)
            launcher.E_OnThrowPointReached -= OnThrowPointReached;
    }

    private void OnThrowPointReached()
    {
        if (launcher.isLastThrowAtEnemy)
            roundStats.throws = roundStats.throws + 1;
        else
            roundStats.passes = roundStats.passes + 1;
    }
    private void OnZeroHp()
    {
        roundStats.deaths = roundStats.deaths + 1;

        PlayerRoundStatsCollector throwerCollector = hp.lastDamager.GetComponent<PlayerRoundStatsCollector>();
        throwerCollector.roundStats.knocks = throwerCollector.roundStats.knocks + 1;
    }
}