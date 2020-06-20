using UnityEngine;
using Photon.Pun;
using Smooth;

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
        ball.E_OnGroundedAfterTime += OnGrounded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        ball.E_OnGroundedAfterTime -= OnGrounded;
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
    private void OnGrounded() => syncer.enabled = true;
}