using UnityEngine;
using Photon.Pun;
using Smooth;
using GW_Lib;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>
{
    PhotonView pv = null;
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        syncer = GetComponent<SmoothSyncPUN2>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        N_GameManager.OnTeamsAreSynced -= OnTeamsAreSynced;
    }

    void Start()
    {
        N_GameManager.OnTeamsAreSynced += OnTeamsAreSynced;
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
    private void OnTeamsAreSynced()
    {
        Debug.LogWarning("WTFFFFFFF");
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("okay, this, should have happened here?");
            GetComponent<DodgeballLaunchUp>().ApplyActionWithCommand = () => false;
            this.SetKinematic(true);
        }
    }
}