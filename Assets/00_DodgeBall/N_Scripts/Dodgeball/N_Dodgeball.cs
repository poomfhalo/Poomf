using UnityEngine;
using Photon.Pun;
using Smooth;
using System;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>
{
    PhotonView pv = null;
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    protected override void Awake()
    {
        base.Awake();
        N_GameManager.OnGameInitialized += OnGameInitialized;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        syncer = GetComponent<SmoothSyncPUN2>();
        GetComponent<DodgeballLaunchUp>().OnLaunchedUp += OnLaunchedUp;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GetComponent<DodgeballLaunchUp>().OnLaunchedUp -= OnLaunchedUp;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        N_GameManager.OnGameInitialized -= OnGameInitialized;
    }

    void FixedUpdate()
    {
        if (ball.IsHeld || ball.IsFlying)
        {
            if (syncer.enabled)
            {
                syncer.enabled = false;
            }
            return;
        }

        if(syncer.enabled == false)
            syncer.enabled = true;
    }
    public int GetHolder()
    {
        int holder = -1;
        if (ball.holder != null)//Only Send commands if the command caller is the local player
        {
            holder = ball.holder.GetComponent<N_PC>().ActorID;
        }
        return holder;
    }

    private void OnLaunchedUp()
    {
        //syncer.enabled = false;
    }
    private void OnGameInitialized()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            GetComponent<DodgeballLaunchUp>().ApplyActionWithCommand = () => false;
        }
    }
}