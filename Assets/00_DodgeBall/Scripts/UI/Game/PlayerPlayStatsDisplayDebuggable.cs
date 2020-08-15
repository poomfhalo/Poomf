using UnityEngine;

public class PlayerPlayStatsDisplayDebuggable : GameDebuggable
{
    [SerializeField] GameObject container = null;
    public override void SetActivity(bool toState)
    {
        container.SetActive(toState);
    }
}
