using System;
using GW_Lib;
using UnityEngine;
using UnityEngine.Events;

public class DodgeballEvents : MonoBehaviour
{
    [Serializable]
    public class DelayedUEvents
    {
        public float delay = 0.5f;
        public UnityEvent e = null;
        public void PlayEvent(MonoBehaviour mono)
        {
            mono.InvokeDelayed(delay, () => e?.Invoke());
        }
    }
    [SerializeField] DelayedUEvents onBallGrounded = null;

    void Awake()
    {
        Dodgeball.instance.E_OnStateUpdated += OnStateUpdated;
    }

    private void OnStateUpdated(Dodgeball.BallState newState)
    {
        switch (newState)
        {
            case Dodgeball.BallState.OnGround:
                onBallGrounded.PlayEvent(this);
                break;
        }
    }
}