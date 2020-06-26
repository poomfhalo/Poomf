using Photon.Pun;
using UnityEngine;

public class N_Dodgeball_D : MonoBehaviour, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(rb3d.position);
        //    stream.SendNext(rb3d.velocity);
        //}
        //else if (stream.IsReading)
        //{
        //    if (firstRead)
        //    {
        //        firstRead = false;
        //        stream.ReceiveNext();
        //        stream.ReceiveNext();
        //        return;
        //    }

        //    netPos = (Vector3)stream.ReceiveNext();
        //    netVel = (Vector3)stream.ReceiveNext();
        //}
        //lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        //netPos = netPos + netVel * lastLag;
    }
    void FixedUpdate()
    {
        //if (photonView.IsMine)
        //    return;
        //if (ball.IsHeld || ball.IsFlying)
           //return;
        //ball.SetKinematic(false);
        //Vector3 posDir = (netPos - rb3d.position).normalized;
        //Vector3 posVel = posDir * catchUpSpeed;
        //Vector3 targetVel = netVel + posVel;
        //Vector3 smoothVel = Vector3.Lerp(rb3d.velocity, targetVel, lastLag * Time.fixedDeltaTime * catchUpSpeed);

        //float dist = Vector3.Distance(rb3d.position, netPos);
        //if (dist <= arrivalDist)
        //{
        //    float pDist = dist / arrivalDist;
        //    smoothVel = smoothVel * pDist;
        //}
        //rb3d.velocity = smoothVel;
    }
}
