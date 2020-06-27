using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class N_DodgeballV2 : MonoBehaviour, IPunObservable
{
    [SerializeField] int commandsSnapCount = 3;
    [SerializeField] List<DodgeballCommand> sentCommands = new List<DodgeballCommand>();

    PhotonView pv = null;
    Dodgeball ball = null;

    void Start()
    {
        ball = GetComponent<Dodgeball>();
        pv = GetComponent<PhotonView>();

        ball.E_OnCommandActivated += OnCommandActivated;
        ball.E_OnStateUpdated += OnStateUpdated;
    }

    private void OnCommandActivated(DodgeballCommand cmd)
    {

    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {

        }
        else if(stream.IsReading)
        {

        }
    }
}