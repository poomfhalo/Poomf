using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoundStatsCollector : MonoBehaviour
{
    public event Action OnDataUpdated = null;
    public PlayerPlayStats GetRoundStats => roundStats;

    [Header("Read Only")]
    [SerializeField] PlayerPlayStats roundStats = null;

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

        PlayerPlayStatsGameHolder holder = PlayerPlayStatsGameHolder.instance;
        if(holder && chara)
            holder.AddStats(chara.GetID(), roundStats);
    }

    private void OnThrowPointReached()
    {
        if (launcher.isLastThrowAtEnemy)
            roundStats.throws = roundStats.throws + 1;
        else
            roundStats.passes = roundStats.passes + 1;

        OnDataUpdated?.Invoke();
    }
    private void OnZeroHp()
    {
        roundStats.deaths = roundStats.deaths + 1;

        PlayerRoundStatsCollector throwerCollector = hp.lastDamager.GetComponent<PlayerRoundStatsCollector>();
        throwerCollector.roundStats.knocks = throwerCollector.roundStats.knocks + 1;

        OnDataUpdated?.Invoke();
        throwerCollector.OnDataUpdated?.Invoke();
    }
}