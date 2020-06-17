using UnityEngine;
using Photon.Pun;
using Smooth;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>
{
    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        rb3d = GetComponent<Rigidbody>();
        syncer = GetComponent<SmoothSyncPUN2>();
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
}